using System;
using System.Collections.ObjectModel;
using AvaloniaEdit.Rendering;
using LiveChartsCore.Defaults;
using ReactiveUI;

namespace PlaywrightTest.ViewModels;

public class PerfTraceViewModel : ViewModelBase {
    private readonly bool _isPath;
    private string _title;
    private string _averageResponseTime;
    private string _p90ResponseTime;
    private string _throughput;
    private bool _isExpanded;
    private string _host;
    private bool _isVisible;

    public PerfTraceViewModel(bool isPath) {
        _isPath = isPath;
        _isExpanded = false;
        _isVisible = false;
    }

    public bool IsVisible {
        get => _isVisible;
        set => this.RaiseAndSetIfChanged(ref _isVisible ,value);
    }

    public bool IsPath => _isPath;

    public bool HasChildren => IsPath;

    public bool IsExpanded {
        get => _isExpanded;
        set => this.RaiseAndSetIfChanged(ref _isExpanded, value);
    }

    public ObservableCollection<PerfTraceViewModel> Children { get; } = new();

    public string Host {
        get => _host;
        set => this.RaiseAndSetIfChanged(ref _host, value);
    }

    public string Title {
        get => _title;
        set => this.RaiseAndSetIfChanged(ref _title, value);
    }

    public string AverageResponseTime {
        get => _averageResponseTime;
        set => this.RaiseAndSetIfChanged(ref _averageResponseTime, value);
    }

    public string P90ResponseTime {
        get => _p90ResponseTime;
        set => this.RaiseAndSetIfChanged(ref _p90ResponseTime, value);
    }

    public string Throughput {
        get => _throughput;
        set => this.RaiseAndSetIfChanged(ref _throughput, value);
    }
    public static int SortP90Ascending(PerfTraceViewModel? x, PerfTraceViewModel? y) {
        double ToDouble(string v) {
            return v == "--" ? 0 : Convert.ToDouble(v);
        }

        return ToDouble(x.P90ResponseTime).CompareTo(ToDouble(y.P90ResponseTime));
    }

    public static int SortP90Descending(PerfTraceViewModel? x, PerfTraceViewModel? y) {
        double ToDouble(string v) {
            return v == "--" ? 0 : Convert.ToDouble(v);
        }

        return ToDouble(y.P90ResponseTime)
            .CompareTo(ToDouble(x.P90ResponseTime));
    }
    public static int SortAveAscending(PerfTraceViewModel? x, PerfTraceViewModel? y) {
        double ToDouble(string v) {
            return v == "--" ? 0 : Convert.ToDouble(v);
        }

        return ToDouble(x.AverageResponseTime).CompareTo(ToDouble(y.AverageResponseTime));
    }

    public static int SortAveDescending(PerfTraceViewModel? x, PerfTraceViewModel? y) {
        double ToDouble(string v) {
            return v == "--" ? 0 : Convert.ToDouble(v);
        }

        return ToDouble(y.AverageResponseTime)
            .CompareTo(ToDouble(x.AverageResponseTime));
    }

    public static int SortThroughputAscending(PerfTraceViewModel? x, PerfTraceViewModel? y) {
        return Convert.ToDouble(x.Throughput)
            .CompareTo(Convert.ToDouble(y.Throughput));
    }

    public static int SortThroughputDescending(PerfTraceViewModel? x, PerfTraceViewModel? y) {
        return Convert.ToDouble(y.Throughput)
            .CompareTo(Convert.ToDouble(x.Throughput));
    }
}
