using System;
using System.Diagnostics;
using System.Timers;
using ReactiveUI;

namespace PlaywrightTest.ViewModels;

public class PanelViewModel : ViewModelBase {
    private int _responseCount;
    private string _virtualUserStatus;
    private int _errorCount;
    private  string _testTimerDisplay;
    private Stopwatch st;
    private readonly Timer _ticker;

    public PanelViewModel() {
        var t = new System.Timers.Timer();
        t.AutoReset = true;
        t.Enabled = false;
        t.Interval = 1000; //1 sec
        t.Elapsed += OnElapsed;
        this._ticker = t;
    }
    public int ResponseCount {
        get => _responseCount;
        set => this.RaiseAndSetIfChanged(ref _responseCount ,value);
    }

    public string VirtualUserStatus {
        get => _virtualUserStatus;
        set => this.RaiseAndSetIfChanged(ref _virtualUserStatus , value);
    }

    public int ErrorCount {
        get => _errorCount;
        set => this.RaiseAndSetIfChanged(ref _errorCount , value);
    }

    public string TestTimerDisplay {
        get => _testTimerDisplay;
        set => this.RaiseAndSetIfChanged(ref _testTimerDisplay , value);
    }

    public void Start() {
        _ticker.Stop();
        _ticker.Start();
        ErrorCount = 0;
        TestTimerDisplay = "starting..";
        VirtualUserStatus = "";
        ResponseCount = 0;
        this.st = Stopwatch.StartNew();

    }

    public void Stop() {
        _ticker.Stop();
    }
    private void OnElapsed(object? sender, ElapsedEventArgs e) {
        this.TestTimerDisplay = st.Elapsed.ToString(@"mm\:ss");
    }
}
