using System.Collections.Generic;

namespace PlaywrightTest.Models;

public class PathTraceSnapshot {
    public PathTraceSnapshot() {
    }

    public double AveRespTime { get; set; }
    public double Throughput { get; set; }
    public double P90RespTime { get; set; }
    public string Path { get; set; }

    public string Host { get; set; }

    public List<PathTraceSnapshotItem> Items { get; } = new();
}