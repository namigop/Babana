using System;
using System.Collections.Generic;
using System.Linq;

namespace PlaywrightTest.Models;

public class PerfPageRequestRunData {
    public PerfPageRequestRunData() {
        StartTime = DateTime.Now;
    }

    public string RunName { get; set; } = $"Run : {DateTime.Now}";
    public DateTime StartTime { get; }
    public DateTime EndTime { get; set; }
    public List<PerfPageRequestPathData> Traces { get; set; } = new();

    public void Add(PerfPageRequestTraceData data) {
        var name = data.Url;
        if (data.Url.StartsWith("http://") || data.Url.StartsWith("https://")) {
            var uri = new Uri(data.Url);
            name = $"{uri.Scheme}://{uri.Host}:{uri.Port}{uri.AbsolutePath}";
        }

        lock (this) {
            var m = Traces.FirstOrDefault(t => t.Path == name);
            if (m == null) {
                m = new PerfPageRequestPathData(name, data.ResourceType);
                Traces.Add(m);
            }

            m.Add(data);
        }
    }

    public List<PerfPageRequestPathData> TakeSnapshot() {
        lock (this)
            return this.Traces.ToList();
    }
}
