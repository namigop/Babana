using System;
using System.Collections.ObjectModel;
using System.Linq;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using PlaywrightTest.Models;
using SkiaSharp;

namespace PlaywrightTest.ViewModels;

public class PerfOverallViewModel : ViewModelBase {
    private readonly ObservableCollection<PerfTraceViewModel> _pathTraces;

    public PerfOverallViewModel(ObservableCollection<PerfTraceViewModel> pathTraces) {
        _pathTraces = pathTraces;
        AllSeries = new ObservableCollection<LineSeries<TimeSpanPoint>>();
    }

    public PerfTraceRunData RunData { get; set; }

    public ObservableCollection<LineSeries<TimeSpanPoint>> AllSeries { get; }

    public Axis[] XAxes { get; set; } = {
        new() {
            Labeler = value => TimeSpan.FromTicks((long)value).ToString(@"mm\:ss"),
            UnitWidth = TimeSpan.FromSeconds(1).Ticks,
            MinStep = TimeSpan.FromSeconds(1).Ticks,
            Name = "Elapsed"
        }
    };

    public Axis[] YAxes { get; set; } = {
        new() {
            UnitWidth = 2,
            MinStep = 2,
            Name = "Response (msec)",
            MinLimit = 0
        }
    };

    public void UpdateOverallChart(string[] traces) {
        //build the overall chart

        foreach (var vm in _pathTraces) {
            var tag = $"{vm.Host}/{vm.Title}";
            var series = AllSeries.FirstOrDefault(s => s.Tag.ToString() == tag);

            if (series == null) {
                series = CreateLineSeries(tag, AllSeries.Count);
                AllSeries.Add(series);
                series.IsVisible = true;
            }

            vm.IsVisible = traces.Contains(tag);
            series.IsVisible = vm.IsVisible;

            var tracePoints = RunData.Traces.FirstOrDefault(t => t.Path == vm.Title).GetTimestampedTraceData();
            var colxn = (ObservableCollection<TimeSpanPoint>)series.Values;

            if (tracePoints.Length > colxn.Count)
                for (var i = colxn.Count; i < tracePoints.Length; i++) {
                    var (timeSpan, elapsedMsec) = tracePoints[i];
                    colxn.Add(new TimeSpanPoint(timeSpan, elapsedMsec));
                }
        }

    }

    private LineSeries<TimeSpanPoint> CreateLineSeries(string tag, int index) {
        var pos = index < DefaultChartColors.Colors.Length ? index : (index + 1) % DefaultChartColors.Colors.Length;
        return new LineSeries<TimeSpanPoint> {
            Tag = tag,
            Name = tag,
            Fill = new SolidColorPaint(DefaultChartColors.Colors[pos].WithAlpha(50)),
            GeometrySize = 4,
            Values = new ObservableCollection<TimeSpanPoint>(),
            Stroke = new SolidColorPaint(DefaultChartColors.Colors[pos], 1.5f),
            GeometryStroke = new SolidColorPaint(DefaultChartColors.Colors[pos], 2),
            LineSmoothness = 0.5
        };
    }
}
