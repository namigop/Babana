using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.CodeAnalysis.CSharp;

namespace PlaywrightTest.Models;

public class PerfRunner {
    private readonly int _durationSec;
    private int _rampupSec;
    private readonly int _virtualUsers;
    private readonly ScriptTabModel _scriptTabModel;
    private Timer _timer;
    private readonly List<PerfVirtualUser> _perfVirtualUsers;

    public PerfRunner(int durationSec, int rampupSec, int virtualUsers, ScriptTabModel scriptTabModel) {
        _perfVirtualUsers = new List<PerfVirtualUser>();
        _durationSec = durationSec;
        _rampupSec = rampupSec;
        _virtualUsers = virtualUsers;
        _scriptTabModel = scriptTabModel;

        _timer = new System.Timers.Timer() {
            Enabled = false,
            AutoReset = false
        };

        _timer.Elapsed += OnTimerElapsed;

    }

    private void OnTimerElapsed(object? sender, ElapsedEventArgs e) {
        this.Stop();
    }

    public async Task Start() {
        if (_virtualUsers <= 0) {
            Console.WriteLine($"Unable to start a test with {_virtualUsers} virtual users");
            return;
        }
        if (_durationSec <= 0) {
            Console.WriteLine($"Unable to start a test with {_durationSec} sec duration");
            return;
        }
        if (_rampupSec <= 0) {
            _rampupSec = 0;
        }

        if (String.IsNullOrWhiteSpace(_scriptTabModel.ScriptContent)) {
            Console.WriteLine($"Unable to start a test with an empty script");
            return;
        }

        Console.WriteLine("Starting performance test with the following parameters");
        Console.WriteLine($"Duration (sec) : {_durationSec}");
        Console.WriteLine($"  Rampup (sec) :{_rampupSec}");
        Console.WriteLine($" Virtual users : {_virtualUsers}");


        _timer.Interval = _durationSec;
        _timer.Start();

        //create and start each virtual user
        _perfVirtualUsers.Clear();
        for (int i = 0; i < _virtualUsers; i++) {
            var vu = new PerfVirtualUser(_scriptTabModel);
            vu.Start();
            _perfVirtualUsers.Add(vu);
            await Task.Delay(_rampupSec);
        }

    }

    public async Task Stop() {
        foreach (var v in _perfVirtualUsers)
            v.Stop();
    }
}
