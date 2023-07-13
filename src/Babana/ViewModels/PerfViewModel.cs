using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Threading;
using PlaywrightTest.Core;
using PlaywrightTest.Models;
using ReactiveUI;

namespace PlaywrightTest.ViewModels;

public class PerfViewModel : ViewModelBase {
    private readonly ScriptTabModel _scriptTabModel;
    private readonly DispatcherTimer _timer;
    private int _durationSec = 60;
    private string _filter = "qpkvc";
    private PerfRunner _perfRunner;
    private int _rampupSec = 10;
    private PerfTraceRunData _runData;

    private int _virtualUsers = 1;
    private PerfPageRequestRunData _runDataBrowser;


    public PerfViewModel(ScriptTabModel scriptTabModel) {
        _scriptTabModel = scriptTabModel;
        StartCommand = CreateCommand(OnStart);
        StopCommand = CreateCommand(OnStop);
        Errors = new ErrorViewModel();
        ReqRespTracer.Instance.Value.Traced += OnTraced;
        PageTracer.Instance.Value.Traced += OnPageTraced;
        MessageHub.Sub += OnMesageReceived;


        _timer = new DispatcherTimer();
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.IsEnabled = false;
        _timer.Tick += OnProcessPerfData;

        PerfResponsesViewModel = new PerfResponsesViewModel(PathTraces);
        PerfOverallViewModel = new PerfOverallViewModel(PathTraces);
        BrowserTraceViewModel = new BrowserTraceViewModel();
    }

    public void Close() {
        ReqRespTracer.Instance.Value.Traced -= OnTraced;
        PageTracer.Instance.Value.Traced -= OnPageTraced;
        MessageHub.Sub -= OnMesageReceived;
        _timer.Tick -= OnProcessPerfData;
        _perfRunner?.Close();
    }

    public BrowserTraceViewModel BrowserTraceViewModel { get; }

    public PerfResponsesViewModel PerfResponsesViewModel { get; }

    public PerfOverallViewModel PerfOverallViewModel { get; }
    public ErrorViewModel Errors { get; }

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

    private void OnPageTraced(object? sender, PerfPageRequestTraceData e) {
        _runDataBrowser.Add(e);
    }

    private void OnMesageReceived(object? sender, Message msg) {
        switch (msg.Content) {
            case ScriptTimeoutMessage info:
                Errors.Add(info.Error);
                break;
            case PerfStatusMessage info:
                IsRunning = info.IsRunning;
                if (!IsRunning)
                    OnStopInternal();
                break;
            //throw new Exception("Unknown message");
        }
    }


    private void OnProcessPerfData(object? sender, EventArgs e) {
        string Format(double val) {
            return val <= 0 ? "--" : $"{val:0.0}";
        }

        if (!IsRunning)
            return;

        //Process browser data
        BrowserTraceViewModel.Add(_runDataBrowser.TakeSnapshot());

        var snapshots = _runData.TakeSnapshot(_runData.VirtualUserCount);
        Console.WriteLine("");

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

        PerfResponsesViewModel.UpdateChartOfSelectedPath();
        PerfResponsesViewModel.UpdateBarSeries();
        PerfOverallViewModel.UpdateOverallChart();
    }


    private void OnTraced(object? sender, ReqRespTraceData trace) {
        if (!IsRunning)
            return;

        if (trace != null) {
            if (!string.IsNullOrWhiteSpace(Filter) && !trace.RequestUri.Contains(Filter)) return;

            if (string.IsNullOrWhiteSpace(trace.StatusCode)) return;

            if (Convert.ToInt32(trace.StatusCode) >= 400) {
                Errors.Add(trace);
                return;
            }

            _runData.Add(PerfTraceData.FromReqRespTrace(trace));
        }
    }

    private async Task OnStop() {
        await OnStopInternal();
        await _perfRunner?.Stop();

        MessageHub.Publish(new PerfStatusMessage { IsRunning = false });
    }

    private async Task OnStopInternal() {
        IsRunning = false;
        _timer.Stop();
    }

    private async Task OnStart() {
        IsRunning = true;
        Errors.Clear();
        PathTraces.Clear();
        PerfOverallViewModel.AllSeries.Clear();
        _runData = new PerfTraceRunData();
        _runDataBrowser = new PerfPageRequestRunData();
        PerfResponsesViewModel.RunData = _runData;
        PerfOverallViewModel.RunData = _runData;
        MessageHub.Publish(new PerfStatusMessage { IsRunning = true });
        _perfRunner = new PerfRunner(_runData, _durationSec, _rampupSec, _virtualUsers, _filter, _scriptTabModel);
        _timer.Start();
        await _perfRunner.Start();
    }
}
