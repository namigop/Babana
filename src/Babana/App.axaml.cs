using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using PlaywrightTest.ViewModels;
using PlaywrightTest.Views;

namespace PlaywrightTest;

public class App : Application {
    public override void Initialize() {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted() {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.MainWindow = new MainWindow {
                DataContext = new MainWindowViewModel()
            };

        base.OnFrameworkInitializationCompleted();
        Startup.Start();
    }

    private void AboutMenuItem_OnClick(object? sender, EventArgs e) {
    }
}