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
    private int _virtualUsers = 1;
    private int _rampupSec = 10;
    private int _durationSec = 60;
    private PerfRunner _perfRunner;
    private PerfTraceRunData _runData;
    private readonly DispatcherTimer _timer;
    private string _filter = "qpkvc";
    private ObservableCollection<ObservableValue> _singleLineValues;


    public PerfViewModel(ScriptTabModel scriptTabModel) {
        _scriptTabModel = scriptTabModel;
        StartCommand = CreateCommand(OnStart);
        StopCommand = CreateCommand(OnStop);
        ReqRespTracer.Instance.Value.Traced += OnTraced;
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
    public Axis[] XAxes { get; set; } =
    {
        new Axis
        {
            Labeler = value => TimeSpan.FromTicks((long)value).ToString(@"mm\:ss"),
            UnitWidth = TimeSpan.FromSeconds(1).Ticks,
            MinStep = TimeSpan.FromSeconds(1).Ticks,
        }
    };

    private void SetupSingleLineSeries() {
        _singleLineValues = new ObservableCollection<ObservableValue>();
        var line = new LineSeries<ObservableValue> {
            Values = _singleLineValues,
            GeometrySize = 4,
            Stroke = new SolidColorPaint(SKColor.FromHsl(216,66,8), 2)
        };

        Series = new ObservableCollection<ISeries> { line };
    }

    private LineSeries<TimeSpanPoint> CreateLineSeries(string tag) {
        return new LineSeries<TimeSpanPoint> {
            Tag = tag,
            Name = tag,
            Fill = null,
            GeometrySize = 4,
            Values = new ObservableCollection<TimeSpanPoint>()
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
                _singleLineValues.Add(new(y));
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

        //build the overall chart
        foreach (var vm in PathTraces) {
            var tag = $"{vm.Host}/{vm.Title}";
            var series = AllSeries.FirstOrDefault(s => s.Tag.ToString() == tag);
            if (series == null) {
                series = CreateLineSeries(tag);
                AllSeries.Add(series);
            }

            var tracePoints = _runData.Traces.FirstOrDefault(t => t.Path == vm.Title).GetTimestampedTraceData();
            var colxn = (ObservableCollection<TimeSpanPoint>)series.Values;

            if (tracePoints.Length > colxn.Count) {
                for (int i = colxn.Count; i < tracePoints.Length; i++) {
                    var (timeSpan, elapsedMsec) = tracePoints[i];
                    colxn.Add(new TimeSpanPoint(timeSpan, elapsedMsec) );
                }
            }

        }
    }

    private void OnTraced(object? sender, ReqRespTraceData trace) {
        if (!IsRunning)
            return;

        if (trace != null && trace.RequestUri.Contains(Filter)) {
            _runData.Add(PerfTraceData.FromReqRespTrace(trace));
        }
    }

    private async Task OnStop() {
        this.IsRunning = false;
        _timer.Stop();
        await _perfRunner?.Stop();
        MessageHub.Publish(new PerfStatusMessage() { IsRunning = false });
    }

    private async Task OnStart() {
        this.IsRunning = true;
        this.PathTraces.Clear();
        this.AllSeries.Clear();
        _runData = new PerfTraceRunData();
        MessageHub.Publish(new PerfStatusMessage() { IsRunning = true });
        _perfRunner = new PerfRunner(_runData, _durationSec, _rampupSec, _virtualUsers, _filter, _scriptTabModel);
        _timer.Start();
        await _perfRunner.Start();
    }

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
}
