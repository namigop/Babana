using System;
using System.ComponentModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;

using PlaywrightTest.Core;
using PlaywrightTest.Models;
using PlaywrightTest.ViewModels;

namespace Babana.Views;

public partial class PerfWindow : Window {
    public PerfWindow(ScriptTabModel model) {
        InitializeComponent();
        DataContext = new PerfViewModel(model, UpdateRowVisibilityBrowser, UpdateRowVisibilityAPI);
        if (Util.IsWindows()) ExtendClientAreaToDecorationsHint = false;

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
                var isVisible = ((PerfTraceViewModel)r.Model).IsVisible;
                if (r.IsVisible != isVisible) {
                    r.IsVisible = isVisible;
                }
            }
        }
    }
    private void UpdateRowVisibilityBrowser() {
        var presenter = this.FindControl<TreeDataGrid>("BrowserTree").RowsPresenter;
        if (presenter != null) {
            var rows = presenter.GetVisualChildren().Cast<TreeDataGridRow>();
            foreach (var r in rows.Where(t => t.Model != null)) {
                var isVisible = ((BrowserPageTraceViewModel)r.Model).IsVisible;
                if (r.IsVisible != isVisible) {
                    r.IsVisible = isVisible;
                }
            }
        }


    }
}
