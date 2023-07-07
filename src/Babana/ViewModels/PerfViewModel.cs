using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Threading;
using PlaywrightTest.Core;
using PlaywrightTest.Models;
using ReactiveUI;

namespace PlaywrightTest.ViewModels;


public class PerfViewModel : ViewModelBase {
    private readonly ScriptTabModel _scriptTabModel;
    private int _virtualUsers = 1;
    private int _rampupSec = 10;
    private int _durationSec = 60;
    private PerfRunner _perfRunner;
    private PerfTraceRunData _runData;
    private readonly DispatcherTimer _timer;

    public PerfViewModel(ScriptTabModel scriptTabModel) {
        _scriptTabModel = scriptTabModel;
        StartCommand = CreateCommand(OnStart);
        StopCommand = CreateCommand(OnStop);
        ReqRespTracer.Instance.Value.Traced += OnTraced;

        _timer = new DispatcherTimer();
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.IsEnabled = false;
        _timer.Tick += OnProcessPerData;
    }

    private void OnProcessPerData(object? sender, EventArgs e) {
       Console.WriteLine($"Processing perf : Count {_runData.Traces.Count}");

    }

    private void OnTraced(object? sender, ReqRespTraceData trace) {
         _runData.Add(PerfTraceData.FromReqRespTrace(trace));
    }

    private async Task OnStop() {
        await _perfRunner?.Stop();
        MessageHub.Publish(new PerfStatusMessage(){ IsRunning = false});
    }

    private async Task OnStart() {
        _runData = new PerfTraceRunData();
       MessageHub.Publish(new PerfStatusMessage(){ IsRunning = true});
       _perfRunner = new PerfRunner(_durationSec, _rampupSec, _virtualUsers, _scriptTabModel);

       await _perfRunner.Start();
    }

    public int VirtualUsers {
        get => _virtualUsers;
        set => this.RaiseAndSetIfChanged(ref _virtualUsers , value);
    }

    public int RampupSec {
        get => _rampupSec;
        set => this.RaiseAndSetIfChanged(ref _rampupSec , value);
    }

    public int DurationSec {
        get => _durationSec;
        set => this.RaiseAndSetIfChanged(ref _durationSec, value);
    }

    public ICommand StartCommand { get; }


    public ICommand StopCommand { get; }
}
