using System.Threading.Tasks;

namespace PlaywrightTest.Models;

public class PerfVirtualUser {
    private readonly ScriptTabModel _scriptTabModel;
    private readonly ScriptRunner _runner;

    public PerfVirtualUser(ScriptTabModel scriptTabModel) {
        _scriptTabModel = scriptTabModel;
        _runner = new ScriptRunner(scriptTabModel);
    }

    public async Task Start() {
       await _runner.Run();
    }

    public async Task Stop() {
        await _runner.ForceClose();
    }
}
