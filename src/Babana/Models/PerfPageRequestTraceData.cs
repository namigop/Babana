using System.Security.AccessControl;
using Microsoft.Playwright;

namespace PlaywrightTest.Models;

public class PerfPageRequestTraceData {
    public PerfPageRequestTraceData(string requestUrl, string resource, RequestTimingResult timing) {
        Url = requestUrl;
        ResourceType = resource;
        TcpHandshakeMsec = Measure(timing.ConnectEnd, timing.ConnectStart);
        DnsLookupMsec = Measure(timing.DomainLookupEnd, timing.DomainLookupStart);
        RequestTimeMsec = Measure(timing.ResponseStart, timing.RequestStart);
        ResponseTimeMsec = Measure(timing.ResponseEnd, timing.ResponseStart);
        TlsNegotiationMsec = Measure(timing.RequestStart, timing.SecureConnectionStart);
        DurationMsec = timing.ResponseEnd;
    }

    static float Measure(float x, float y) {
        if (x == -1 || y == -1)
            return -1;
        return x - y;
    }

    public string ResourceType { get; }
    public float ResponseTimeMsec { get; }

    public float DurationMsec { get; }

    public float TlsNegotiationMsec { get; }

    public float RequestTimeMsec { get; }

    public float DnsLookupMsec { get; }

    public float TcpHandshakeMsec { get; }

    public string Url { get; }
}
