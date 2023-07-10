using System;
using System.Collections.Generic;

namespace PlaywrightTest.Models;

public record ReqRespTraceData {
    public byte[] Screenshot { get; set; }
    public string ScreenshotName { get; set; }
    public uint ElapsedMsec { get; set; }
    public DateTime Timestamp { get; set; }
    public string RequestBody { get; set; }
    public string ResponseBody { get; set; }
    public Dictionary<string, string> RequestHeaders { get; set; }
    public Dictionary<string, string> ResponseHeaders { get; set; }
    public string RequestUri { get; set; }
    public string StatusCode { get; set; }
    public string DocId { get; set; }
    public DateTime LastUpdated { get; set; }
    public string RequestMethod { get; set; }
}