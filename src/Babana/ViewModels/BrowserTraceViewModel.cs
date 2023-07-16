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
                    new TemplateColumn<BrowserPageTraceViewModel>("Url",
                        "UrlTemplate",
                        new GridLength(400, GridUnitType.Pixel)),
                    x => x.Children,
                    x => x.HasChildren,
                    x => x.IsExpanded
                ),
                // new TextColumn<BrowserPageTraceViewModel, string>(
                //  "Url",
                //  x => x.Url,
                //  new GridLength(400, GridUnitType.Pixel)),
                new TemplateColumn<BrowserPageTraceViewModel>(
                    "Type",
                    "ResourceTypeTemplate"),
                new TemplateColumn<BrowserPageTraceViewModel>(
                    "Duration",
                    "DurationTemplate",
                    new GridLength(1, GridUnitType.Auto),
                    new TextColumnOptions<BrowserPageTraceViewModel> {
                        CompareAscending = BrowserPageTraceViewModel.SortDurationAscending,
                        CompareDescending = BrowserPageTraceViewModel.SortDurationDescending
                    }),
                new TemplateColumn<BrowserPageTraceViewModel>(
                    "Dns Lookup",
                    "DnsLookupTemplate",
                    new GridLength(1, GridUnitType.Auto)),
                new TemplateColumn<BrowserPageTraceViewModel>(
                    "Tcp Handshake",
                    "TcpHandshakeTemplate"),
                new TemplateColumn<BrowserPageTraceViewModel>(
                    "TLS Negotiation",
                    "TlsNegotiationTemplate"
                ),
                new TemplateColumn<BrowserPageTraceViewModel>(
                    "Request",
                    "RequestTimeTemplate"
                ),
                new TemplateColumn<BrowserPageTraceViewModel>(
                    "Response",
                    "ResponseTimeTemplate"
                )
            }
        };
    }

    public HierarchicalTreeDataGridSource<BrowserPageTraceViewModel> PageTraceTree { get; set; }

    public void Add(List<PerfPageRequestPathData> pathData, int topItemsCount) {
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

        int count = 0;
        foreach (var t in PageTraces.OrderByDescending(t => t.Children.FirstOrDefault(x => x.Name == "P90").DurationMsec).ToArray()) {
            t.IsVisible = count < topItemsCount;
            count++;
        }
    }

    public void Clear() {
        PageTraces.Clear();
    }
}
