using System;
using System.Collections.ObjectModel;
using AvaloniaEdit.Rendering;
using LiveChartsCore.Defaults;
using ReactiveUI;

namespace PlaywrightTest.ViewModels;

public class PerfTraceViewModel : ViewModelBase {
    private readonly bool _isPath;
    private string _title;
    private float _averageResponseTime;
    private float _p90ResponseTime;
    private float _throughput;
    private bool _isExpanded;
    private string _host;
    private bool _isVisible;
    private int _p90ProgressValue;
    private string _p90Background;

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

    public float AverageResponseTime {
        get => _averageResponseTime;
        set => this.RaiseAndSetIfChanged(ref _averageResponseTime, value);
    }

    public float P90ResponseTime {
        get => _p90ResponseTime;
        set => this.RaiseAndSetIfChanged(ref _p90ResponseTime, value);
    }

    public float Throughput {
        get => _throughput;
        set => this.RaiseAndSetIfChanged(ref _throughput, value);
    }

    public int P90ProgressValue {
        get => _p90ProgressValue;
        set => this.RaiseAndSetIfChanged(ref _p90ProgressValue , value);
    }

    public string P90Background {
        get => _p90Background;
        set => this.RaiseAndSetIfChanged(ref _p90Background, value);
    }

    public static int SortP90Ascending(PerfTraceViewModel? x, PerfTraceViewModel? y) {
        return x.P90ResponseTime.CompareTo(y.P90ResponseTime);
    }

    public static int SortP90Descending(PerfTraceViewModel? x, PerfTraceViewModel? y) {
        return y.P90ResponseTime.CompareTo(x.P90ResponseTime);
    }
    public static int SortAveAscending(PerfTraceViewModel? x, PerfTraceViewModel? y) {
        return x.AverageResponseTime.CompareTo(y.AverageResponseTime);
    }

    public static int SortAveDescending(PerfTraceViewModel? x, PerfTraceViewModel? y) {
        return y.AverageResponseTime.CompareTo(x.AverageResponseTime);
    }

    public static int SortThroughputAscending(PerfTraceViewModel? x, PerfTraceViewModel? y) {
        return x.Throughput.CompareTo(y.Throughput);
    }

    public static int SortThroughputDescending(PerfTraceViewModel? x, PerfTraceViewModel? y) {
        return y.Throughput.CompareTo(x.Throughput);
    }
}
