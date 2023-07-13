using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.CodeAnalysis.CSharp;

namespace PlaywrightTest.Models;

public class PerfRunner {
    private readonly PerfTraceRunData _perfTraceRunData;
    private readonly int _durationSec;
    private int _rampupSec;
    private readonly int _virtualUsers;
    private readonly string _filter;
    private readonly ScriptTabModel _scriptTabModel;
    private Timer _stopTestTimer;
    private readonly List<PerfVirtualUser> _perfVirtualUsers;
    private bool _canStart;

    public PerfRunner(PerfTraceRunData perfTraceRunData, int durationSec, int rampupSec, int virtualUsers, string filter, ScriptTabModel scriptTabModel) {
        _perfVirtualUsers = new List<PerfVirtualUser>();
        _perfTraceRunData = perfTraceRunData;
        _durationSec = durationSec;
        _rampupSec = rampupSec;
        _virtualUsers = virtualUsers;
        _filter = filter;
        _scriptTabModel = scriptTabModel;

        _stopTestTimer = new Timer() {
            Enabled = false,
            AutoReset = false
        };

        _canStart = true;
        _stopTestTimer.Elapsed += OnTimerElapsed;
    }

    public void Close() {
        _stopTestTimer.Elapsed -= OnTimerElapsed;
    }
    private void OnTimerElapsed(object? sender, ElapsedEventArgs e) {
        Stop();
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

        if (_rampupSec <= 0) _rampupSec = 0;

        if (string.IsNullOrWhiteSpace(_scriptTabModel.ScriptContent)) {
            Console.WriteLine($"Unable to start a test with an empty script");
            return;
        }

        _canStart = true;
        Console.WriteLine("Starting performance test with the following parameters");
        Console.WriteLine($"Duration (sec) : {_durationSec}");
        Console.WriteLine($"  Rampup (sec) :{_rampupSec}");
        Console.WriteLine($" Virtual users : {_virtualUsers}");


        _stopTestTimer.Interval = _durationSec * 1000;
        _stopTestTimer.Stop();
        _stopTestTimer.Start();

        //create and start each virtual user
        _perfVirtualUsers.Clear();
        for (var i = 0; i < _virtualUsers; i++) {
            if (!_canStart) {
                break;
            }

            var vu = new PerfVirtualUser(_scriptTabModel);
            _perfTraceRunData.VirtualUserCount += 1;
            _perfVirtualUsers.Add(vu);

            vu.Start();
            await Task.Delay(_rampupSec * 1000);
        }
    }

    public async Task Stop() {
        _canStart = false;
        foreach (var v in _perfVirtualUsers)
            v.Stop();
    }
}
