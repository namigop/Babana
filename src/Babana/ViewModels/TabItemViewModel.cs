using PlaywrightTest.Models;
using ReactiveUI;

namespace PlaywrightTest.ViewModels;

public class TabItemViewModel : ViewModelBase {
    private ScriptTabModel _model;

    public ScriptTabModel Model {
        get => _model;
        set => this.RaiseAndSetIfChanged(ref _model, value);
    }

    public TabItemViewModel(ScriptTabModel model) {
        Model = model;
    }
}
