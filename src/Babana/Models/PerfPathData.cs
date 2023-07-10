using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;

namespace PlaywrightTest.Models;

public class PerfPathData {
    public string Path { get; }
    private readonly List<PerfTraceData> _traces;
    private double _aveRespTime;
    private double _throughput;
    private double _p90RespTime; //todo

    private readonly DateTime _startTime;

    private PathTraceSnapshot _snapshot;

    public PerfPathData(string path, string host, DateTime startTime) {
        Path = path;
        Host = host;
        _traces = new List<PerfTraceData>();
        _aveRespTime = 0.0;
        _throughput = 0.0;
        _startTime = startTime;
        _snapshot = new PathTraceSnapshot() { Path = path, Host = host };
    }

    public string Host { get; }

    public void Add(PerfTraceData data) {
        var prevTotalTime = _aveRespTime * _traces.Count;
        var newTotalTime = prevTotalTime + data.ElapsedMsec;
        _traces.Add(data);

        var span = (DateTime.Now - _startTime).TotalSeconds;
        _aveRespTime = newTotalTime / _traces.Count;
        _throughput = _traces.Count * 1.0 / span;
    }

    private static uint CalculatePercentile(uint[] points, int percentile) {
        var pt = points.Length * (percentile / 100.0);
        var index = (int)Math.Round(pt, MidpointRounding.ToZero);
        return points[index];
    }

    public PathTraceSnapshot TakeSnapshot(int userCount) {
        _snapshot.AveRespTime = _aveRespTime;
        _snapshot.Throughput = _throughput;
        var item = _snapshot.Items.FirstOrDefault(i => i.VirtualUserCount == userCount);
        if (item != null) {
            item.AveRespTime = _aveRespTime;
            item.Throughput = _throughput;
        }
        else {
            item = new PathTraceSnapshotItem();
            item.VirtualUserCount = userCount;
            item.AveRespTime = _aveRespTime;
            item.Throughput = _throughput;
            _snapshot.Items.Add(item);
        }

        _snapshot.P90RespTime = CalculatePercentile(_traces.Select(t => t.ElapsedMsec).Order().ToArray(), 90);
        item.P90RespTime = _snapshot.P90RespTime;

        return _snapshot;
    }

    public uint[] GetTraceData() {
        return _traces.Select(t => t.ElapsedMsec).ToArray();
    }

    public (TimeSpan, uint ElapsedMsec)[] GetTimestampedTraceData() {
        return _traces.Select(t => (t.Timestamp - _startTime, t.ElapsedMsec)).ToArray();
    }
}
