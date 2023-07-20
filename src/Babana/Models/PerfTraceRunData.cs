using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Avalonia.Controls.Shapes;
using DynamicData;

namespace PlaywrightTest.Models;

public class PerfTraceRunData {
    private static string[] PathTemplates = new[] {
        "/v2/ufone/en_us-UFONE/webfront/cxos-oms/orders/{orderRef}",
        "/v2/ufone/en_us-UFONE/webfront/numbers/{number}/lock"
    };

    private static List<Tuple<string, Regex>> PathTemplatesRegex = PathTemplates.Select(t => ConvertToRegex(t)).ToList();

    private static Tuple<string, Regex> ConvertToRegex(string template) {
        var start = template.IndexOf("{");
        var end = template.IndexOf("}");
        var pattern = Regex.Replace(template, @"{\S+}", @"\S+");
        return Tuple.Create(template, new Regex(pattern));
    }

    public PerfTraceRunData() {
        StartTime = DateTime.Now;
    }

    public string RunName { get; set; } = $"Run : {DateTime.Now}";
    public DateTime StartTime { get; }
    public DateTime EndTime { get; set; }
    public List<PerfPathData> Traces { get; set; } = new();
    public int VirtualUserCount { get; set; }


    public void Add(PerfTraceData data) {
        if (data == null)
            return;

        PerfPathData p;
        lock (this) {
            var templatedPath = GetTemplatedPath(data.RequestUriPath);
            p = Traces.FirstOrDefault(t => t.Path == templatedPath);
            if (p == null) {
                p = new PerfPathData(templatedPath, new Uri(data.RequestUri).Host, StartTime);
                Traces.Add(p);
            }
        }

        p.Add(data);
    }


    private string GetTemplatedPath(string dataRequestUriPath) {
        foreach (var r in PathTemplatesRegex) {
            var isMatch = r.Item2.IsMatch(dataRequestUriPath);
            if (isMatch)
                return r.Item1;
        }

        return dataRequestUriPath;
    }

    public List<PathTraceSnapshot> TakeSnapshot(int userCount) {
        return
            Traces.Select(t => t.TakeSnapshot(userCount))
                .ToList();
    }

    public int GetResponseCount() {
        return Traces.Select(t => t.GetTraceDataCount()).Sum();
    }
}
