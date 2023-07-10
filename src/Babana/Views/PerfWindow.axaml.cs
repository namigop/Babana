using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PlaywrightTest.Models;
using PlaywrightTest.ViewModels;

namespace Babana.Views;

public partial class PerfWindow : Window {
    public PerfWindow(ScriptTabModel model) {
        InitializeComponent();
        DataContext = new PerfViewModel(model);


#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void OnSelectionChanging(object? sender, CancelEventArgs e) {
    }

    public PerfWindow() {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }
}