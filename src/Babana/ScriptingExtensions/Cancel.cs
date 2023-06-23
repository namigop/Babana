using System.Threading;

namespace PlaywrightTest.ScriptingExtensions;

public class Cancel {
    private readonly CancellationToken _token;

    public Cancel(CancellationToken token) {
        _token = token;
    }

    public void TryCancel() {
        _token.ThrowIfCancellationRequested();
    }
}