using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Xml;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Diagnostics;
using Avalonia.Interactivity;
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

        this.Editor.TextChanged += OnTextChanged;
        this.Activated += OnActivated;
        SetupSyntaxHighlighting();
        var intercept = new ConsoleIntercept(this.LogEditor);
        Console.SetOut(intercept);

#if DEBUG
        this.AttachDevTools(new DevToolsOptions());
#endif
        MessageHub.Sub += (sender, arg) => { this.LogEditor.ScrollToEnd(); };
    }



    private void SetupSyntaxHighlighting() {
        using var resource = typeof(TextEditor).Assembly.GetManifestResourceStream("AvaloniaEdit.Highlighting.Resources.CSharp-Mode.xshd");
        if (resource != null) {
            using var reader = new XmlTextReader(resource);
            Editor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
        }
    }

    public MainWindowViewModel ViewModel => (MainWindowViewModel)this.DataContext;

    private void OnActivated(object? sender, EventArgs e) {
        if (this.Editor.Document.TextLength == 0) {
            this.Editor.Document = new TextDocument() { Text = ViewModel.ScriptViewModel.Model.ScriptContent };
        }
    }

    private void OnTextChanged(object? sender, EventArgs e) {
        ViewModel.ScriptViewModel.Model.ScriptContent = this.Editor.Document.Text;
        if (!this.ViewModel.Hello.EndsWith("*")) {
            this.ViewModel.Hello += "*";
        }
    }

    private async void OnOpenFileClick(object? sender, RoutedEventArgs e) {
        var dg = new OpenFileDialog();
        dg.AllowMultiple = false;
        dg.Title = "Select a script file (*.csx)";
        dg.Filters = new List<FileDialogFilter>() {
            new() { Extensions = new List<string>() { "csx" }, Name = "csx" }
        };

        var files = await dg.ShowAsync(this);
        if ((bool)files?.Any()) {

            this.ViewModel.ScriptViewModel.Model.FromFile(files[0]);
            this.ViewModel.Hello = System.IO.Path.GetFileName(files[0]);
            this.ViewModel.MyFortuneCookie = files[0];
            this.Editor.Document.Text = this.ViewModel.ScriptViewModel.Model.ScriptContent;
            this.ViewModel.Hello = this.ViewModel.Hello.TrimEnd('*');
        }

    }
}
