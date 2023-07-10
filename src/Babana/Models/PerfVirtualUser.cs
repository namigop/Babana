using System.Threading.Tasks;
using Avalonia.Controls;

namespace PlaywrightTest.Models;

public class PerfVirtualUser {
    private readonly ScriptTabModel _scriptTabModel;
    private ScriptRunner _runner;
    private bool _canStart;

    public PerfVirtualUser(ScriptTabModel scriptTabModel) {
        _scriptTabModel = scriptTabModel;
        _canStart = true;
    }

    public async Task Start() {
        while (_canStart) {
            _runner = new ScriptRunner(_scriptTabModel);
            await _runner.Run();
            _runner.ForceClose();
        }
    }

    public async Task Stop() {
        _canStart = false;
        await _runner.ForceClose();
    }
}
