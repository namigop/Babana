using System;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Selection;
using Avalonia.Threading;
using DynamicData;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using PlaywrightTest.Core;
using PlaywrightTest.Models;
using ReactiveUI;
using SkiaSharp;

namespace PlaywrightTest.ViewModels;

public class PerfViewModel : ViewModelBase {
    private readonly ScriptTabModel _scriptTabModel;
    private readonly DispatcherTimer _timer;
    private int _durationSec = 60;
    private string _filter = "qpkvc";
    private PerfRunner _perfRunner;
    private int _rampupSec = 10;
    private PerfTraceRunData _runData;
    private ObservableCollection<ObservableValue> _singleLineValues;
    private int _virtualUsers = 1;


    public PerfViewModel(ScriptTabModel scriptTabModel) {
        _scriptTabModel = scriptTabModel;
        StartCommand = CreateCommand(OnStart);
        StopCommand = CreateCommand(OnStop);
        this.Errors = new ErrorViewModel();
        ReqRespTracer.Instance.Value.Traced += OnTraced;
        MessageHub.Sub += OnMesageReceived;

        PathTracesTree = new HierarchicalTreeDataGridSource<PerfTraceViewModel>(PathTraces) {
            Columns = {
                new HierarchicalExpanderColumn<PerfTraceViewModel>(
                    new TextColumn<PerfTraceViewModel, string>("", x => x.Title),
                    x => x.Children,
                    x => x.HasChildren,
                    x => x.IsExpanded),
                new TextColumn<PerfTraceViewModel, string>(
                    "Host",
                    x => x.Host
                ),
                new TextColumn<PerfTraceViewModel, string>(
                    "Average (msec)",
                    x => x.AverageResponseTime,
                    new GridLength(1, GridUnitType.Auto),
                    new TextColumnOptions<PerfTraceViewModel>() {
                        CompareAscending = PerfTraceViewModel.SortAveAscending,
                        CompareDescending = PerfTraceViewModel.SortAveDescending
                    }),
                new TextColumn<PerfTraceViewModel, string>(
                    "P90 (msec)",
                    x => x.P90ResponseTime),
                new TextColumn<PerfTraceViewModel, string>(
                    "Throughput (resp/sec)",
                    x => x.Throughput,
                    new GridLength(1, GridUnitType.Auto),
                    new TextColumnOptions<PerfTraceViewModel>() {
                        CompareAscending = PerfTraceViewModel.SortThroughputAscending,
                        CompareDescending = PerfTraceViewModel.SortThroughputDescending
                    })
            }
        };

        PathTracesTree.RowSelection.SingleSelect = true;
        PathTracesTree.RowSelection.SelectionChanged += OnSelectionChanged;

        _timer = new DispatcherTimer();
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.IsEnabled = false;
        _timer.Tick += OnProcessPerfData;

        SetupSingleLineSeries();
        SetupMultiLineSeries();
    }

    private void OnMesageReceived(object? sender, Message msg) {
        switch (msg.Content) {
            case ScriptTimeoutMessage info:
                Errors.Add(info.Error);
                break;
            //throw new Exception("Unknown message");
        }
    }

    public ErrorViewModel Errors { get; }

    public Axis[] YAxes { get; set; } = {
        new() {
            UnitWidth = 10,
            MinStep = 50,
            Name = "Response (msec)",
            MinLimit = 0
        }
    };

    public Axis[] XAxes { get; set; } = {
        new() {
            Labeler = value => TimeSpan.FromTicks((long)value).ToString(@"mm\:ss"),
            UnitWidth = TimeSpan.FromSeconds(1).Ticks,
            MinStep = TimeSpan.FromSeconds(1).Ticks,
            Name = "Elapsed"
        }
    };

    public bool IsRunning { get; set; }

    public ObservableCollection<PerfTraceViewModel> PathTraces { get; } = new();

    public int VirtualUsers {
        get => _virtualUsers;
        set => this.RaiseAndSetIfChanged(ref _virtualUsers, value);
    }

    public int RampupSec {
        get => _rampupSec;
        set => this.RaiseAndSetIfChanged(ref _rampupSec, value);
    }

    public int DurationSec {
        get => _durationSec;
        set => this.RaiseAndSetIfChanged(ref _durationSec, value);
    }

    public ICommand StartCommand { get; }


    public ICommand StopCommand { get; }

    public string Filter {
        get => _filter;
        set => this.RaiseAndSetIfChanged(ref _filter, value);
    }

    public HierarchicalTreeDataGridSource<PerfTraceViewModel> PathTracesTree { get; }
    public ObservableCollection<ISeries> Series { get; private set; }
    public ObservableCollection<LineSeries<TimeSpanPoint>> AllSeries { get; private set; }

    private void SetupSingleLineSeries() {
        _singleLineValues = new ObservableCollection<ObservableValue>();
        var line = new LineSeries<ObservableValue> {
            Values = _singleLineValues,
            GeometrySize = 3,
            Stroke = new SolidColorPaint(SKColor.FromHsl(216, 66, 8), 2),
            LineSmoothness = 0.5
        };

        Series = new ObservableCollection<ISeries> { line };
    }

    private LineSeries<TimeSpanPoint> CreateLineSeries(string tag, int index) {
        var pos = index < DefaultChartColors.Colors.Length ? index : (index + 1) % DefaultChartColors.Colors.Length;
        return new LineSeries<TimeSpanPoint> {
            Tag = tag,
            Name = tag,
            Fill = null,
            GeometrySize = 4,
            Values = new ObservableCollection<TimeSpanPoint>(),
            Stroke = new SolidColorPaint(DefaultChartColors.Colors[pos], 1.5f),
            GeometryStroke = new SolidColorPaint(DefaultChartColors.Colors[pos], 2),
            LineSmoothness = 0.5
        };
    }

    private void SetupMultiLineSeries() {
        AllSeries = new ObservableCollection<LineSeries<TimeSpanPoint>>();
    }

    private void OnSelectionChanged(object? sender, TreeSelectionModelSelectionChangedEventArgs<PerfTraceViewModel> e) {
        var selectedItem = e.SelectedItems.FirstOrDefault();

        if (selectedItem != null) {
            if (!selectedItem.IsPath)
                return;

            var path = _runData.Traces.First(t => t.Path == selectedItem.Title);
            var ypoints = path.GetTraceData();
            _singleLineValues.Clear();
            foreach (var y in ypoints)
                _singleLineValues.Add(new ObservableValue(y));
        }
    }


    private void OnProcessPerfData(object? sender, EventArgs e) {
        string Format(double val) {
            return val <= 0 ? "--" : $"{val:0.0}";
        }

        var snapshots = _runData.TakeSnapshot(_runData.VirtualUserCount);
        Console.WriteLine($"");

        foreach (var snap in snapshots) {
            var trace = PathTraces.FirstOrDefault(t => t.Title == snap.Path);
            if (trace == null) {
                trace = new PerfTraceViewModel(true);
                PathTraces.Add(trace);
            }

            trace.Host = snap.Host;
            trace.Throughput = Format(snap.Throughput);
            trace.Title = snap.Path;
            trace.AverageResponseTime = Format(snap.AveRespTime);
            trace.P90ResponseTime = Format(snap.P90RespTime);

            foreach (var item in snap.Items) {
                var title = $"#Users : {item.VirtualUserCount}";
                var traceItem = trace.Children.FirstOrDefault(t => t.Title == title);
                if (traceItem == null) {
                    traceItem = new PerfTraceViewModel(false) { Title = title };
                    trace.Children.Add(traceItem);
                }

                traceItem.Throughput = Format(item.Throughput);
                traceItem.AverageResponseTime = Format(item.AveRespTime);
                traceItem.P90ResponseTime = Format(item.P90RespTime);
            }
        }

        UpdateChartOfSelectedPath();
        UpdateOverallChart();
    }

    private void UpdateOverallChart() {
        //build the overall chart
        foreach (var vm in PathTraces) {
            var tag = $"{vm.Host}/{vm.Title}";
            var series = AllSeries.FirstOrDefault(s => s.Tag.ToString() == tag);
            if (series == null) {
                series = CreateLineSeries(tag, AllSeries.Count);
                AllSeries.Add(series);
            }

            var tracePoints = _runData.Traces.FirstOrDefault(t => t.Path == vm.Title).GetTimestampedTraceData();
            var colxn = (ObservableCollection<TimeSpanPoint>)series.Values;

            if (tracePoints.Length > colxn.Count)
                for (var i = colxn.Count; i < tracePoints.Length; i++) {
                    var (timeSpan, elapsedMsec) = tracePoints[i];
                    colxn.Add(new TimeSpanPoint(timeSpan, elapsedMsec));
                }
        }
    }

    private void UpdateChartOfSelectedPath() {
        //build the chart for the selected path
        var currentSelectedPath = PathTracesTree.RowSelection.SelectedItem;
        if (currentSelectedPath != null) {
            var currentTrace = _runData.Traces.FirstOrDefault(t => t.Path == currentSelectedPath.Title);
            var currentPoints = currentTrace.GetTraceData();
            if (currentPoints.Length > _singleLineValues.Count)
                for (var i = _singleLineValues.Count; i < currentPoints.Length; i++)
                    _singleLineValues.Add(new ObservableValue(currentPoints[i]));
        }
    }

    private void OnTraced(object? sender, ReqRespTraceData trace) {
        if (!IsRunning)
            return;

        if (trace != null) {
            if (!string.IsNullOrWhiteSpace(Filter) && !trace.RequestUri.Contains(Filter)) {
                return;
            }

            if (string.IsNullOrWhiteSpace(trace.StatusCode)) {
                return;
            }
            if (Convert.ToInt32(trace.StatusCode) >= 400) {
                this.Errors.Add(trace);
                return;
            }

            _runData.Add(PerfTraceData.FromReqRespTrace(trace));
        }
    }

    private async Task OnStop() {
        IsRunning = false;
        _timer.Stop();
        await _perfRunner?.Stop();
        MessageHub.Publish(new PerfStatusMessage() { IsRunning = false });
    }

    private async Task OnStart() {
        IsRunning = true;
        Errors.Clear();
        PathTraces.Clear();
        AllSeries.Clear();
        _runData = new PerfTraceRunData();
        MessageHub.Publish(new PerfStatusMessage() { IsRunning = true });
        _perfRunner = new PerfRunner(_runData, _durationSec, _rampupSec, _virtualUsers, _filter, _scriptTabModel);
        _timer.Start();
        await _perfRunner.Start();
    }
}
