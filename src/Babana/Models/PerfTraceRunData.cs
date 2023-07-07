using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls.Shapes;
using DynamicData;

namespace PlaywrightTest.Models;

public class PerfPathData {
    public string Path { get; }
    private readonly List<PerfTraceData> _traces;
    private double _aveRespTime;
    private double _throughput;
    private readonly DateTime _startTime;

    public PerfPathData(string path) {
        Path = path;
        _traces = new List<PerfTraceData>();
        _aveRespTime = 0.0;
        _throughput = 0.0;
        _startTime = DateTime.Now;
    }

    public void Add(PerfTraceData data) {
        var prevTotalTime = _aveRespTime * _traces.Count;
        var newTotalTime = prevTotalTime + data.ElapsedMsec;

        _traces.Add(data);

        _aveRespTime = newTotalTime / _traces.Count;
        _throughput = _traces.Count / (DateTime.Now - _startTime).TotalSeconds;

        Console.WriteLine($"{Path}");
        Console.WriteLine($"    Ave: {_aveRespTime}");
        Console.WriteLine($"    Throughput: {_throughput}");
    }
}

public class PerfTraceRunData {
    public string RunName { get; set; } = $"Run : {DateTime.Now}";
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public List<PerfPathData> Traces { get; set; } = new();

    public void Add(PerfTraceData data) {
        if (data == null)
            return;

        PerfPathData p;
        lock (this) {
            p = this.Traces.FirstOrDefault(t => t.Path == data.RequestUriPath);
            if (p == null) {
                p = new PerfPathData(data.RequestUriPath);
                Traces.Add(p);
            }
        }

        p.Add(data);
    }
}
