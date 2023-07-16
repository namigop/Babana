using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Diagnostics;
using Avalonia.Interactivity;
using Avalonia.Threading;
using AvaloniaEdit;
using AvaloniaEdit.Document;
using AvaloniaEdit.Highlighting;
using AvaloniaEdit.Highlighting.Xshd;
using PlaywrightTest.Core;
using PlaywrightTest.ViewModels;

namespace PlaywrightTest.Views;

public partial class MainWindow : Window {
    public MainWindow() {
        InitializeComponent();

        Editor.TextChanged += OnTextChanged;
        Activated += OnActivated;
        SetupSyntaxHighlighting();
        var intercept = new ConsoleIntercept(LogEditor);
        Console.SetOut(intercept);

#if DEBUG
        this.AttachDevTools(new DevToolsOptions());
#endif
        MessageHub.Sub += (_, _) => { Dispatcher.UIThread.Post(LogEditor.ScrollToEnd); };
        if (Util.IsWindows()) ExtendClientAreaToDecorationsHint = false;
    }


    private void SetupSyntaxHighlighting() {
        using var resource = typeof(TextEditor).Assembly.GetManifestResourceStream("AvaloniaEdit.Highlighting.Resources.CSharp-Mode.xshd");
        if (resource != null) {
            using var reader = new XmlTextReader(resource);
            Editor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
        }
    }

    public MainWindowViewModel ViewModel => (MainWindowViewModel)DataContext;

    private void OnActivated(object? sender, EventArgs e) {
        if (Editor.Document.TextLength == 0) Editor.Document = new TextDocument() { Text = ViewModel.ScriptViewModel.Model.ScriptContent };
    }

    private void OnTextChanged(object? sender, EventArgs e) {
        ViewModel.ScriptViewModel.Model.ScriptContent = Editor.Document.Text;
        if (!ViewModel.Hello.EndsWith("*")) ViewModel.Hello += "*";
    }

    private async void OnOpenFileClick(object? sender, RoutedEventArgs e) {
        var dg = new OpenFileDialog();
        dg.AllowMultiple = false;
        dg.Title = "Select a script file (*.csx)";
        dg.Filters = new List<FileDialogFilter>() {
            new() { Extensions = new List<string>() { "csx" }, Name = "csx" }
        };

        var files = await dg.ShowAsync(this);
        if (files != null && files.Any()) {
            ViewModel.ScriptViewModel.Model.FromFile(files[0]);
            ViewModel.Hello = System.IO.Path.GetFileName(files[0]);
            ViewModel.MyFortuneCookie = files[0];
            Editor.Document.Text = ViewModel.ScriptViewModel.Model.ScriptContent;
            ViewModel.Hello = ViewModel.Hello.TrimEnd('*');
        }
    }
}
