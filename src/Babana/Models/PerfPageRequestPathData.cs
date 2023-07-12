using System;
using System.Collections.Generic;
using System.Linq;

namespace PlaywrightTest.Models;

public class PerfPageRequestPathData {
    public PerfPageRequestPathData(string path, string resourceType) {
        Path = path;
        ResourceType = resourceType;
    }

    public string Path { get; }
    public List<Measurement> Trace { get; } = new();
    public string ResourceType { get; }

    public void Add(PerfPageRequestTraceData data) {
        // var name = data.Url;
        // if (data.Url.StartsWith("http://") || data.Url.StartsWith("https://")) {
        //     var uri = new Uri(data.Url);
        //     name = $"{uri.Scheme}://{uri.Host}:{uri.Port}{uri.AbsolutePath}";
        // }
        //
        void TryCreate(string name, Func<float> get) {
            var m = Trace.FirstOrDefault(t => t.Name == name);
            if (m == null) {
                m = new Measurement(name);
                m.Add(get());
                Trace.Add(m);
            }
            else {
                m.Add(get());
            }
        }
        TryCreate(nameof(data.DurationMsec), () => data.DurationMsec);
        TryCreate(nameof(data.DnsLookupMsec), () => data.DnsLookupMsec);
        TryCreate(nameof(data.ResponseTimeMsec), () => data.ResponseTimeMsec);
        TryCreate(nameof(data.RequestTimeMsec), () => data.RequestTimeMsec);
        TryCreate(nameof(data.TcpHandshakeMsec), () => data.TcpHandshakeMsec);
        TryCreate(nameof(data.TlsNegotiationMsec), () => data.TlsNegotiationMsec);
    }

}
