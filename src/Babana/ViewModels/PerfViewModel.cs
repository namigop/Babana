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
    private readonly Action _updateRowVisibilityBrowser;
    private readonly Action _updateRowVisibilityApi;
    private readonly DispatcherTimer _timer;
    private int _durationSec = 60;
    private string _filter = "qpkvc";
    private PerfRunner _perfRunner;
    private int _rampupSec = 10;
    private PerfTraceRunData _runData;

    private int _virtualUsers = 1;
    private PerfPageRequestRunData _runDataBrowser;
    private int _topItemsCount = 10;
    private bool _isRunning;
    private int _currentVirtualUsers;
    private readonly PanelViewModel _panelViewModel;
    private readonly BrowserTraceViewModel _browserTraceViewModel;
    private readonly PerfResponsesViewModel _perfResponsesViewModel;
    private readonly PerfOverallViewModel _perfOverallViewModel;
    private readonly ErrorViewModel _errors;
    private readonly ObservableCollection<PerfTraceViewModel> _pathTraces = new();
    private readonly ICommand _startCommand;
    private readonly ICommand _stopCommand;
    private bool _hasData;


    public PerfViewModel(ScriptTabModel scriptTabModel, Action updateRowVisibilityBrowser, Action updateRowVisibilityApi) {
        _scriptTabModel = scriptTabModel;
        _updateRowVisibilityBrowser = updateRowVisibilityBrowser;
        _updateRowVisibilityApi = updateRowVisibilityApi;
        _startCommand = CreateCommand(OnStart);
        _stopCommand = CreateCommand(OnStop);
        _errors = new ErrorViewModel();
        ReqRespTracer.Instance.Value.Traced += OnTraced;
        PageTracer.Instance.Value.Traced += OnPageTraced;
        MessageHub.Sub += OnMesageReceived;


        _timer = new DispatcherTimer();
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.IsEnabled = false;
        _timer.Tick += OnProcessPerfData;

        _perfResponsesViewModel = new PerfResponsesViewModel(PathTraces);
        _perfOverallViewModel = new PerfOverallViewModel(PathTraces);
        _browserTraceViewModel = new BrowserTraceViewModel();
        _panelViewModel = new PanelViewModel();
    }

    public PanelViewModel PanelViewModel => _panelViewModel;


    public void Close() {
        ReqRespTracer.Instance.Value.Traced -= OnTraced;
        PageTracer.Instance.Value.Traced -= OnPageTraced;
        MessageHub.Sub -= OnMesageReceived;
        _timer.Tick -= OnProcessPerfData;
        _perfRunner?.Close();
    }

    public BrowserTraceViewModel BrowserTraceViewModel => _browserTraceViewModel;

    public PerfResponsesViewModel PerfResponsesViewModel => _perfResponsesViewModel;

    public PerfOverallViewModel PerfOverallViewModel => _perfOverallViewModel;

    public ErrorViewModel Errors => _errors;

    public bool IsRunning {
        get => _isRunning;
        set => this.RaiseAndSetIfChanged(ref _isRunning, value);
    }

    public ObservableCollection<PerfTraceViewModel> PathTraces => _pathTraces;

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

    public ICommand StartCommand => _startCommand;


    public ICommand StopCommand => _stopCommand;

    public string Filter {
        get => _filter;
        set => this.RaiseAndSetIfChanged(ref _filter, value);
    }

    public int TopItemsCount {
        get => _topItemsCount;
        set {
            this.RaiseAndSetIfChanged(ref _topItemsCount, value);
            if (!IsRunning) {
                UpdateChartsAndTables();
            }
        }
    }

    public int CurrentVirtualUsers {
        get => _currentVirtualUsers;
        set => this.RaiseAndSetIfChanged(ref _currentVirtualUsers, value);
    }

    public bool HasData {
        get => _hasData;
        private set => this.RaiseAndSetIfChanged(ref _hasData, value);
    }

    private void OnPageTraced(object? sender, PerfPageRequestTraceData e) {
        _runDataBrowser?.Add(e);
    }

    private void OnMesageReceived(object? sender, Message msg) {
        switch (msg.Content) {
            case ScriptTimeoutMessage info:
                Errors.Add(info.Error);
                break;
            case PerfStatusMessage info:
                IsRunning = info.IsRunning;
                CurrentVirtualUsers = info.CurrentVirtualUsers;
                if (!IsRunning)
                    OnStopInternal();
                break;
            //throw new Exception("Unknown message");
        }
    }


    private void OnProcessPerfData(object? sender, EventArgs e) {
        if (!IsRunning)
            return;

        //Process browser data
        BrowserTraceViewModel.Add(_runDataBrowser.TakeSnapshot(), TopItemsCount);

        var snapshots = _runData.TakeSnapshot(_runData.VirtualUserCount);
        Console.WriteLine("");

        foreach (var snap in snapshots) {
            var trace = PathTraces.FirstOrDefault(t => t.Title == snap.Path);
            if (trace == null) {
                trace = new PerfTraceViewModel(true);
                PathTraces.Add(trace);
            }

            trace.Host = snap.Host;
            trace.Throughput = snap.Throughput;
            trace.Title = snap.Path;
            trace.AverageResponseTime = snap.AveRespTime;
            trace.P90ResponseTime = snap.P90RespTime;

            foreach (var item in snap.Items) {
                var title = $"#Users : {item.VirtualUserCount}";
                var traceItem = trace.Children.FirstOrDefault(t => t.Title == title);
                if (traceItem == null) {
                    traceItem = new PerfTraceViewModel(false) { Title = title };
                    trace.Children.Add(traceItem);
                }

                traceItem.Throughput = item.Throughput;
                traceItem.AverageResponseTime = item.AveRespTime;
                traceItem.P90ResponseTime = item.P90RespTime;
            }
        }


        UpdatePanels();
        UpdateChartsAndTables();
    }

    private void UpdatePanels() {
        PanelViewModel.ErrorCount = Errors.Errors.Count;
        PanelViewModel.VirtualUserStatus = $"{CurrentVirtualUsers} / {VirtualUsers}";
        PanelViewModel.ResponseCount = _runData.GetResponseCount();
        this.HasData = !string.IsNullOrWhiteSpace(PanelViewModel.TestTimerDisplay);
    }

    private void UpdateChartsAndTables() {
        //top slowest APIs
        var top = PathTraces
            .OrderByDescending(t => t.P90ResponseTime)
            .Take(TopItemsCount)
            .Select(vm => $"{vm.Host}/{vm.Title}")
            .ToArray();

        PerfOverallViewModel.UpdateOverallChart(top);
        PerfResponsesViewModel.UpdateBarSeries(TopItemsCount);

        _updateRowVisibilityApi();
        _updateRowVisibilityBrowser();
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

            _runData?.Add(PerfTraceData.FromReqRespTrace(trace));
        }
    }

    private async Task OnStop() {

        await OnStopInternal();
        await _perfRunner?.Stop();

        MessageHub.Publish(new PerfStatusMessage { IsRunning = false });
    }

    private async Task OnStopInternal() {
        IsRunning = false;
        PanelViewModel.Stop();
        _timer.Stop();
    }

    private async Task OnStart() {
        IsRunning = true;
        Errors.Clear();
        PathTraces.Clear();
        PerfOverallViewModel.AllSeries.Clear();
        BrowserTraceViewModel.Clear();
        PanelViewModel.Start();

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
