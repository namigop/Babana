using System.Collections.Generic;

namespace PlaywrightTest.Models;

public class PathTraceSnapshot {
    public PathTraceSnapshot() {
    }

    public float AveRespTime { get; set; }
    public float Throughput { get; set; }
    public float P90RespTime { get; set; }
    public string Path { get; set; }

    public string Host { get; set; }

    public List<PathTraceSnapshotItem> Items { get; } = new();
}
