using System;
using System.Reactive.Linq;
using System.Xml;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using AvaloniaEdit;
using AvaloniaEdit.Document;
using AvaloniaEdit.Highlighting;
using AvaloniaEdit.Highlighting.Xshd;
using PlaywrightTest.Core;
using PlaywrightTest.Models;
using PlaywrightTest.ViewModels;
using ReactiveUI;

namespace PlaywrightTest.Views;

public partial class ReqRespTraceControl : UserControl {
    private readonly TextEditor _reqEditor;
    private readonly TextEditor _respEditor;
    private bool _canAddTrace;

    public ReqRespTraceControl() {
        InitializeComponent();

        ViewModel = new ReqRespTraceViewModel();
        DataContext = ViewModel;
        ReqRespTracer.Instance.Value.Traced += OnTraced;
        MessageHub.Sub += OnSubscribed;
        _canAddTrace = true;

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

    private void OnSubscribed(object? sender, Message msg) {
        switch (msg.Content) {
            case PerfStatusMessage info:
                _canAddTrace = !info.IsRunning;
                break;
            //throw new Exception("Unknown message");
        }
    }

    private void DisplayReqResp() {
        if (ViewModel.SelectedTraceItem != null)
            Dispatcher.UIThread.Post(() => {
                _reqEditor.Document = new TextDocument() { Text = ViewModel.SelectedTraceItem.RequestBody };
                _respEditor.Document = new TextDocument() { Text = ViewModel.SelectedTraceItem.ResponseBody };
            });
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
        if (!_canAddTrace)
            return;

        Dispatcher.UIThread.Post(() => {
            var vm = (ReqRespTraceViewModel)DataContext;
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
