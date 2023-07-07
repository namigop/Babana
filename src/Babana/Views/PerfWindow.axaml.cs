using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PlaywrightTest.Models;
using PlaywrightTest.ViewModels;

namespace Babana.Views; 

public partial class PerfWindow : Window {
    public PerfWindow(ScriptTabModel model) {
        InitializeComponent();
        this.DataContext = new PerfViewModel(model);
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
    }
}
