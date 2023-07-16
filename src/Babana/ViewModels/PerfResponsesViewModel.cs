using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Animation.Animators;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Selection;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using PlaywrightTest.Models;
using ReactiveUI;
using SkiaSharp;

namespace PlaywrightTest.ViewModels;

public class PerfResponsesViewModel : ViewModelBase {
    private readonly ObservableCollection<PerfTraceViewModel> _pathTraces;
    private ObservableCollection<ObservableValue> _singleLineValues;
    private string _selectedTracePath;

    public PerfResponsesViewModel(ObservableCollection<PerfTraceViewModel> pathTraces) {
        _pathTraces = pathTraces;
        this.BarSeries = new();

        PathTracesTree = new HierarchicalTreeDataGridSource<PerfTraceViewModel>(pathTraces) {
            Columns = {
                new HierarchicalExpanderColumn<PerfTraceViewModel>(
                    //  new TextColumn<PerfTraceViewModel, string>("", x => x.Title, new GridLength(400, GridUnitType.Pixel)),
                    new TemplateColumn<PerfTraceViewModel>("Path", "NameTemplate", new GridLength(400, GridUnitType.Pixel)) {
                        IsTextSearchEnabled = true,
                        TextSearchValueSelector = x => x.Title
                    },
                    x => x.Children,
                    x => x.HasChildren,
                    x => x.IsExpanded),
                new TemplateColumn<PerfTraceViewModel>(
                    "Average",
                    "AverageTemplate",
                    new GridLength(1, GridUnitType.Auto),
                    new TextColumnOptions<PerfTraceViewModel> {
                        CompareAscending = PerfTraceViewModel.SortAveAscending,
                        CompareDescending = PerfTraceViewModel.SortAveDescending
                    }),
                new TemplateColumn<PerfTraceViewModel>("p90", "P90Template", new GridLength(1, GridUnitType.Auto),
                    new TextColumnOptions<PerfTraceViewModel> {
                        CompareAscending = PerfTraceViewModel.SortP90Ascending,
                        CompareDescending = PerfTraceViewModel.SortP90Descending
                    }),
                new TemplateColumn<PerfTraceViewModel>(
                    "Throughput (resp/sec)",
                    "ThroughputTemplate",
                    new GridLength(1, GridUnitType.Auto),
                    new TextColumnOptions<PerfTraceViewModel> {
                        CompareAscending = PerfTraceViewModel.SortThroughputAscending,
                        CompareDescending = PerfTraceViewModel.SortThroughputDescending
                    }),
                new TemplateColumn<PerfTraceViewModel>(
                    "Host",
                    "HostTemplate"
                ),
            }
        };

        PathTracesTree.RowSelection.SingleSelect = true;
        PathTracesTree.RowSelection.SelectionChanged += OnSelectionChanged;
        SetupSingleLineSeries();
    }

    public Axis[] YAxes { get; set; } = {
        new() {
            UnitWidth = 2,
            MinStep = 2,
            Name = "Response (msec)",
            MinLimit = 0
        }
    };

    public PerfTraceRunData RunData { get; set; }

    public HierarchicalTreeDataGridSource<PerfTraceViewModel> PathTracesTree { get; }
    public ObservableCollection<ISeries> Series { get; private set; }
    public ObservableCollection<RowSeries<ObservableValue>> BarSeries { get; }
    public Axis[] BarYAxes { get; } = { new() { IsVisible = false } };

    public Axis[] BarXAxes { get; } = {
        new Axis {
            Name = "Response (msec)",
            SeparatorsPaint = new SolidColorPaint(new SKColor(220, 220, 220))
        }
    };

    private void SetupSingleLineSeries() {
        _singleLineValues = new ObservableCollection<ObservableValue>();
        var line = new LineSeries<ObservableValue> {
            Values = _singleLineValues,
            GeometrySize = 3,
            Stroke = new SolidColorPaint(SKColor.FromHsl(216, 66, 8), 2),
            LineSmoothness = 0.5
        };

        Series = new ObservableCollection<ISeries> { line };
    }

    private void OnSelectionChanged(object? sender, TreeSelectionModelSelectionChangedEventArgs<PerfTraceViewModel> e) {
        var selectedItem = e.SelectedItems.FirstOrDefault();

        if (selectedItem != null) {
            if (!selectedItem.IsPath)
                return;

            this.SelectedTracePath = selectedItem.Title;
            var path = RunData.Traces.First(t => t.Path == selectedItem.Title);
            var ypoints = path.GetTraceData();
            _singleLineValues.Clear();
            foreach (var y in ypoints)
                _singleLineValues.Add(new ObservableValue(y));
        }
    }

    public string SelectedTracePath {
        get => _selectedTracePath;
        private set => this.RaiseAndSetIfChanged(ref _selectedTracePath, value);
    }

    public void UpdateChartOfSelectedPath() {
        //build the chart for the selected path
        var currentSelectedPath = PathTracesTree.RowSelection.SelectedItem;
        if (currentSelectedPath != null) {
            var currentTrace = RunData.Traces.FirstOrDefault(t => t.Path == currentSelectedPath.Title);
            if (currentTrace != null) {
                var currentPoints = currentTrace.GetTraceData();
                if (currentPoints.Length > _singleLineValues.Count)
                    for (var i = _singleLineValues.Count; i < currentPoints.Length; i++)
                        _singleLineValues.Add(new ObservableValue(currentPoints[i]));
            }
        }
    }

    private static object key = new object();

    public void UpdateBarSeries(int count) {
        var top = _pathTraces
            .OrderByDescending(t => Convert.ToDouble(t.P90ResponseTime))
            .Take(count)
            .ToArray();

        if (top.Any()) {
            var highest = Convert.ToDouble(top[0].P90ResponseTime);
            for (int i = 0; i < top.Length; i++) {
                top[i].P90ProgressValue = Convert.ToInt32((100.0 * Convert.ToDouble(top[i].P90ResponseTime)) / highest);

            }
        }


        // //remove ones that are no longer in the top {count}
        // var seriesNames = BarSeries.Select(t => t.Name).ToArray();
        // foreach (var title in seriesNames) {
        //     if (top.FirstOrDefault(x => x.Title == title) == null) {
        //         BarSeries.Remove(BarSeries.First(x => x.Name == title));
        //     }
        // }
        //
        //
        // var pos = BarSeries.Count == DefaultChartColors.Colors.Length ? 0 : BarSeries.Count;
        // foreach (var s in top) {
        //     var existing = BarSeries.FirstOrDefault(t => t.Name == s.Title);
        //     if (existing == null) {
        //         var r = new RowSeries<ObservableValue> {
        //             Values = new[] { new ObservableValue(Convert.ToDouble(s.P90ResponseTime)) },
        //             Tag = s.Title,
        //             Name = s.Title,
        //             Stroke = null,
        //             //MaxBarWidth = 25,
        //             Fill = new SolidColorPaint(DefaultChartColors.Colors[pos]),
        //             DataLabelsPaint = new SolidColorPaint(new SKColor(245, 245, 245)),
        //             DataLabelsSize = 11,
        //             //DataLabelsPaint = new SolidColorPaint(SKColors.DimGray),
        //             DataLabelsPosition = DataLabelsPosition.End,
        //             DataLabelsTranslate = new LvcPoint(-1, 0),
        //             DataLabelsFormatter = point => $"{point.Context.Series.Name} {point.PrimaryValue}"
        //         };
        //         this.BarSeries.Add(r);
        //         pos = pos < (DefaultChartColors.Colors.Length - 1) ? pos + 1 : 0;
        //     }
        //     else {
        //         existing.Values.First().Value = Convert.ToDouble(s.P90ResponseTime);
        //     }
        // }
        //
        //
        // //re-sort the bars
        // var sorted = BarSeries.OrderByDescending(t => t.Values.First().Value).ToArray();
        // var pos2 = 0;
        // foreach (var s in sorted) {
        //     BarSeries.Remove(s);
        //     BarSeries.Insert(pos2, s);
        //     pos2 += 1;
        // }
    }
}
