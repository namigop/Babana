using System;
using System.Reactive.Linq;
using System.Xml;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using AvaloniaEdit;
using AvaloniaEdit.Document;
using AvaloniaEdit.Highlighting;
using AvaloniaEdit.Highlighting.Xshd;
using PlaywrightTest.Models;
using PlaywrightTest.ViewModels;
using ReactiveUI;

namespace PlaywrightTest.Views;

public partial class ReqRespTraceControl : UserControl {
    private readonly TextEditor _reqEditor;
    private readonly TextEditor _respEditor;

    public ReqRespTraceControl() {
        InitializeComponent();

        this.ViewModel = new ReqRespTraceViewModel();
        this.DataContext = this.ViewModel;
        ReqRespTracer.Instance.Value.Traced += OnTraced;

        _reqEditor = this.FindControl<TextEditor>("ReqEditor");
        _respEditor = this.FindControl<TextEditor>("RespEditor");
        SetupSyntaxHighlighting(_respEditor);
        SetupSyntaxHighlighting(_reqEditor);

        this.WhenAnyValue(
                t => t.ViewModel.SelectedTraceItem
            )
            .Throttle(TimeSpan.FromMilliseconds(250))
            .Subscribe(t => DisplayReqResp());
    }

    private void DisplayReqResp() {
        if (this.ViewModel.SelectedTraceItem != null) {
            Dispatcher.UIThread.Post(() => {
                _reqEditor.Document = new TextDocument() { Text = ViewModel.SelectedTraceItem.RequestBody };
                _respEditor.Document = new TextDocument() { Text = ViewModel.SelectedTraceItem.ResponseBody };
            });
        }
    }

    public ReqRespTraceViewModel ViewModel { get; private set; }

    private void SetupSyntaxHighlighting(TextEditor editor) {
        using var resource = typeof(TextEditor).Assembly.GetManifestResourceStream("AvaloniaEdit.Highlighting.Resources.Json.xshd");
        if (resource != null) {
            using var reader = new XmlTextReader(resource);
            editor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
        }
    }

    private void OnTraced(object sender, ReqRespTraceData e) {
        Dispatcher.UIThread.Post(() => {
            var vm = (ReqRespTraceViewModel)this.DataContext;
            vm.AddTrace(e);
        });
    }

    public void Close() {
        ReqRespTracer.Instance.Value.Traced -= OnTraced;
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }
}
