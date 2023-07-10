using System;
using System.IO;
using ReactiveUI;

namespace PlaywrightTest.Models;

public class ScriptTabModel : ReactiveObject {
    private string _scriptContent;
    private string _scriptFile;
    private string _scriptName;

    private string _errors;
    private TimeSpan _elapsed;

    public ScriptTabModel() {
    }

    public void FromFile(string file) {
        ScriptContent = File.ReadAllText(file);
        ScriptName = Path.GetFileName(file);
        ScriptFile = file;
    }

    public void FromText(string script, string name) {
        ScriptContent = script;
        ScriptName = name;
    }

    public string ScriptContent {
        get => _scriptContent;
        set => this.RaiseAndSetIfChanged(ref _scriptContent, value);
    }

    public string ScriptFile {
        get => _scriptFile;
        private set => _scriptFile = value;
    }

    public string ScriptName {
        get => _scriptName;
        set => this.RaiseAndSetIfChanged(ref _scriptName, value);
    }

    public TimeSpan Elapsed {
        get => _elapsed;
        set => _elapsed = value;
    }
}