using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Playwright;

namespace PlaywrightTest.Models;

public class PageTracer {
    public event EventHandler<PerfPageRequestTraceData> Traced;

    private PageTracer() {
    }

    public static Lazy<PageTracer> Instance = new(() => new PageTracer());

    public void Trace(string requestUrl, string resourceType, RequestTimingResult timing) {
        var ts = new PerfPageRequestTraceData(requestUrl, resourceType, timing);
        Traced?.Invoke(this, ts);
    }
}
