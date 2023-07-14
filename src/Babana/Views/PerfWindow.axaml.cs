using System;
using System.ComponentModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using PlaywrightTest.Models;
using PlaywrightTest.ViewModels;

namespace Babana.Views;

public partial class PerfWindow : Window {
    public PerfWindow(ScriptTabModel model) {
        InitializeComponent();
        DataContext = new PerfViewModel(model, UpdateRowVisibilityBrowser, UpdateRowVisibilityAPI);


#if DEBUG
        this.AttachDevTools();
#endif
    }


    public PerfWindow() {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
        //this.Closed += OnClosed
    }

    protected override void OnClosed(EventArgs e) {
        base.OnClosed(e);
        ((PerfViewModel)this.DataContext).Close();
    }

    private void UpdateRowVisibilityAPI() {
        var presenter = this.FindControl<TreeDataGrid>("ApiTree").RowsPresenter;
        if (presenter != null) {
            var rows = presenter.GetVisualChildren().Cast<TreeDataGridRow>();
            foreach (var r in rows.Where(t => t.Model != null)) {
                r.IsVisible = ((PerfTraceViewModel)r.Model).IsVisible;
            }
        }
    }
    private void UpdateRowVisibilityBrowser() {
        var presenter = this.FindControl<TreeDataGrid>("BrowserTree").RowsPresenter;
        if (presenter != null) {
            var rows = presenter.GetVisualChildren().Cast<TreeDataGridRow>();
            foreach (var r in rows.Where(t => t.Model != null)) {
                r.IsVisible = ((BrowserPageTraceViewModel)r.Model).IsVisible;
            }
        }


    }
}
