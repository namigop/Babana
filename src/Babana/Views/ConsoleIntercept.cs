using System;
using System.IO;
using System.Text;
using Avalonia.Threading;
using AvaloniaEdit;

namespace PlaywrightTest.Views;

public class ConsoleIntercept : TextWriter {
    private const int Capacity = 500_000;
    private readonly StringBuilder _sb = new();
    private readonly TextEditor _editor;

    public override Encoding Encoding { get; } = Encoding.UTF8;

    public ConsoleIntercept(TextEditor editor) {
        _editor = editor;
    }

    public override void Write(char value) {
        Dispatcher.UIThread.Post(() => {
            lock (Console.Out) {
                if (value == (char)27) {
                    //ESC char
                    _editor.AppendText(" ");
                    return;
                }

                _editor.AppendText(value.ToString());
            }
        });
    }

    public override void WriteLine(string value) {
        Dispatcher.UIThread.Post(() => {
            lock (Console.Out) {
                _editor.AppendText(value);
                _editor.AppendText(Environment.NewLine);
            }
        });
    }
}