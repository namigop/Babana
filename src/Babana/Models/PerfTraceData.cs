using System;

namespace PlaywrightTest.Models;

public class PerfTraceData {
    public uint ElapsedMsec { get; set; }
    public DateTime Timestamp { get; set; }
    public string RequestUri { get; set; }
    public string StatusCode { get; set; }
    public string RequestMethod { get; set; }
    public string RequestUriPath { get; private set; }

    public static PerfTraceData FromReqRespTrace(ReqRespTraceData trace) {
        if (string.IsNullOrWhiteSpace(trace.RequestUri))
            return null;

        return new PerfTraceData() {
            ElapsedMsec = trace.ElapsedMsec,
            Timestamp = trace.Timestamp,
            RequestUri = trace.RequestUri,
            StatusCode = trace.StatusCode,
            RequestMethod = trace.RequestMethod,
            RequestUriPath = new Uri(trace.RequestUri).AbsolutePath
        };
    }
}