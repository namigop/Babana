namespace PlaywrightTest.Models;

public class PathTraceSnapshotItem {
    public double AveRespTime { get; set; }
    public double Throughput { get; set; }
    public double P90RespTime { get; set; }
    public int VirtualUserCount { get; set; }
}