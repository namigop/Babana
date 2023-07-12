using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using DynamicData;
using PlaywrightTest.Models;

namespace PlaywrightTest.ViewModels;

public class BrowserTraceViewModel : ViewModelBase {
    public ObservableCollection<BrowserPageTraceViewModel> PageTraces { get; } = new();

    public BrowserTraceViewModel() {
        PageTraceTree = new HierarchicalTreeDataGridSource<BrowserPageTraceViewModel>(PageTraces) {
            Columns = {
                new HierarchicalExpanderColumn<BrowserPageTraceViewModel>(
                    new TextColumn<BrowserPageTraceViewModel, string>("Url", x => x.Name, new GridLength(400, GridUnitType.Pixel)),
                    x => x.Children,
                    x => x.HasChildren,
                    x => x.IsExpanded
                ),
                // new TextColumn<BrowserPageTraceViewModel, string>(
                //  "Url",
                //  x => x.Url,
                //  new GridLength(400, GridUnitType.Pixel)),
                new TextColumn<BrowserPageTraceViewModel, string>(
                    "Type",
                    x => x.ResourceType),
                new TextColumn<BrowserPageTraceViewModel, float>(
                    "Duration (msec)",
                    x => x.DurationMsec),
                new TextColumn<BrowserPageTraceViewModel, float>(
                    "Dns Lookup (msec)",
                    x => x.DnsLookupMsec),
                new TextColumn<BrowserPageTraceViewModel, float>(
                    "Tcp Handshake (msec)",
                    x => x.TcpHandshakeMsec),
                new TextColumn<BrowserPageTraceViewModel, float>(
                    "TLS Negotiation (msec)",
                    x => x.TlsNegotiationMsec
                ),
                new TextColumn<BrowserPageTraceViewModel, float>(
                    "Request (msec)",
                    x => x.RequestTimeMsec
                ),
                new TextColumn<BrowserPageTraceViewModel, float>(
                    "Response (msec)",
                    x => x.ResponseTimeMsec
                )
            }
        };
    }

    public HierarchicalTreeDataGridSource<BrowserPageTraceViewModel> PageTraceTree { get; set; }

    public void Add(PerfPageRequestTraceData perfPageRequestData) {
        this.PageTraces.Add(BrowserPageTraceViewModel.From(perfPageRequestData));
    }

    public void Add(List<PerfPageRequestPathData> pathData) {
        foreach (var p in pathData) {
            var vm = PageTraces.FirstOrDefault(t => t.Name == p.Path);
            if (vm == null) {
                vm = BrowserPageTraceViewModel.From(p);
                PageTraces.Add(vm);
            }
            else {
                BrowserPageTraceViewModel.UpdateValues(p, vm);
                //update the values
            }
        }
    }
}
