using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PlaywrightTest.Models;
using ReactiveUI;

namespace PlaywrightTest.ViewModels;

public class BrowserPageTraceViewModel : ViewModelBase {
    private float _responseTimeMsec = -1;
    private string _resourceType;
    private float _durationMsec = -1;
    private float _tlsNegotiationMsec = -1;
    private float _requestTimeMsec = -1;
    private float _dnsLookupMsec = -1;
    private float _tcpHandshakeMsec = -1;
    private string _url;
    private readonly ObservableCollection<BrowserPageTraceViewModel> _children = new();
    private bool _hasChildren = false;
    private bool _isExpanded = false;

    public float ResponseTimeMsec {
        get => _responseTimeMsec;
        set => this.RaiseAndSetIfChanged(ref _responseTimeMsec, value);
    }

    public string ResourceType {
        get => _resourceType;
         set => this.RaiseAndSetIfChanged(ref _resourceType, value);
    }

    public float DurationMsec {
        get => _durationMsec;
        set => this.RaiseAndSetIfChanged(ref _durationMsec, value);
    }

    public float TlsNegotiationMsec {
        get => _tlsNegotiationMsec;
        set => this.RaiseAndSetIfChanged(ref _tlsNegotiationMsec, value);
    }

    public float RequestTimeMsec {
        get => _requestTimeMsec;
        set => this.RaiseAndSetIfChanged(ref _requestTimeMsec, value);
    }

    public float DnsLookupMsec {
        get => _dnsLookupMsec;
        set => this.RaiseAndSetIfChanged(ref _dnsLookupMsec, value);
    }

    public float TcpHandshakeMsec {
        get => _tcpHandshakeMsec;
        set => this.RaiseAndSetIfChanged(ref _tcpHandshakeMsec, value);
    }

    public string Name {
        get => _url;
        set => this.RaiseAndSetIfChanged(ref _url, value);
    }

    public ObservableCollection<BrowserPageTraceViewModel> Children => _children;

    public bool HasChildren {
        get => _children.Any();
    }

    public bool IsExpanded {
        get => _isExpanded;
        set => this.RaiseAndSetIfChanged(ref _isExpanded, value);
    }

    public bool IsVisible { get; set; }

    public static BrowserPageTraceViewModel From(PerfPageRequestTraceData data) {
        return new BrowserPageTraceViewModel() {
            Name = data.Url,
            ResourceType = data.ResourceType,
            ResponseTimeMsec = data.ResponseTimeMsec,
            DurationMsec = data.DurationMsec,
            TlsNegotiationMsec = data.TlsNegotiationMsec,
            RequestTimeMsec = data.RequestTimeMsec,
            DnsLookupMsec = data.DnsLookupMsec,
            TcpHandshakeMsec = data.TcpHandshakeMsec
        };
    }

    public static BrowserPageTraceViewModel From(PerfPageRequestPathData data) {
        var m = new BrowserPageTraceViewModel() {
            Name = data.Path,
            ResourceType = data.ResourceType
        };

        UpdateValues(data, m);
        return m;
    }

    private static readonly List<Func<Measurement, float>> getFunctions = new List<Func<Measurement, float>>() {
        m => m.Min,
        m => m.Average,
        m => m.P90,
        m => m.Max,
    };
    static Measurement temp;
    private static readonly string[] subItemNames = new[] { nameof(temp.Min), nameof(temp.Average), nameof(temp.P90), nameof(temp.Max) };

    public static void UpdateValues(PerfPageRequestPathData data, BrowserPageTraceViewModel parentVm) {
        int counter = 0;
        foreach (var name in subItemNames) {
            var vm = parentVm.Children.FirstOrDefault(c => c.Name == name);
            if (vm == null) {
                vm = new BrowserPageTraceViewModel() {
                    Name = name //ave,min,max
                };
                parentVm.Children.Add(vm);
            }

            var get = getFunctions[counter];
            foreach (var measurement in data.Trace) {
                if (measurement.Name == nameof(vm.DnsLookupMsec)) {
                    vm.DnsLookupMsec = get(measurement);
                }

                if (measurement.Name == nameof(vm.DurationMsec)) {
                    vm.DurationMsec = get(measurement);
                }

                if (measurement.Name == nameof(vm.RequestTimeMsec)) {
                    vm.RequestTimeMsec = get(measurement);
                }

                if (measurement.Name == nameof(vm.ResponseTimeMsec)) {
                    vm.ResponseTimeMsec = get(measurement);
                }

                if (measurement.Name == nameof(vm.TcpHandshakeMsec)) {
                    vm.TcpHandshakeMsec = get(measurement);
                }

                if (measurement.Name == nameof(vm.TlsNegotiationMsec)) {
                    vm.TlsNegotiationMsec = measurement.Average;
                }
            }

            counter += 1;
        }

        parentVm.DnsLookupMsec = parentVm.Children.First(c => c.Name == nameof(temp.P90)).DnsLookupMsec;
        parentVm.DurationMsec = parentVm.Children.First(c => c.Name == nameof(temp.P90)).DurationMsec;
        parentVm.RequestTimeMsec = parentVm.Children.First(c => c.Name == nameof(temp.P90)).RequestTimeMsec;
        parentVm.ResponseTimeMsec = parentVm.Children.First(c => c.Name == nameof(temp.P90)).ResponseTimeMsec;
        parentVm.TlsNegotiationMsec = parentVm.Children.First(c => c.Name == nameof(temp.P90)).TlsNegotiationMsec;
        parentVm.TcpHandshakeMsec = parentVm.Children.First(c => c.Name == nameof(temp.P90)).TcpHandshakeMsec;
    }

    private static BrowserPageTraceViewModel From(Measurement data) {
        return new BrowserPageTraceViewModel() {
            Name = data.Name, //Duration/ResponseTime etc.
        };
    }
}
