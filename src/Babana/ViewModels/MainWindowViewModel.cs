﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Babana.Views;
using PlaywrightTest.Core;
using PlaywrightTest.Models;
using PlaywrightTest.Views;
using ReactiveUI;

namespace PlaywrightTest.ViewModels;

public class MainWindowViewModel : ViewModelBase {
    private bool _canResume;
    private bool _canStart;
    private TabItemViewModel _selectedTabItem;
    private ScriptRunner? _runner;
    private string _toastBackground;
    private object _toastMessage;
    private string _hello = $"Hello {Environment.UserName}!";
    private string _myFortuneCookie = FortuneCookie.GetMyFortune();

    public MainWindowViewModel() : base() {
        StartCommand = CreateCommand(OnStart);
        StopCommand = CreateCommand(OnStop);
        ForceCloseCommand = CreateCommand(OnForceClose);
        OpenFileCommand = CreateCommand(OnOpenFile);
        SaveFileCommand = CreateCommand(OnSave);
        ResumeCommand = CreateCommand(OnResume);
        ClearTracesCommand = CreateCommand(OnClearTraces);
        SaveTraceCommand = CreateCommand(OnSaveTrace);
        InstallBrowserCommand = CreateCommand(OnInstallBrowser);
        ScreenshotCommand = CreateCommand(OnScreenshot);
        RunPerfCommand = CreateCommand(OnRunPerf);
        var model = new ScriptTabModel();
        model.FromText("//Click the \"Open\" menu to select a *.csx script", "TODO");
        ScriptViewModel = new TabItemViewModel(model);
        CanStart = true;

        MessageHub.Sub += OnSubscribed;
    }


    private void OnSubscribed(object sender, Message msg) {
        switch (msg.Content) {
            case RunStateMessage info:
                HandleRunStates(info);
                break;
            //throw new Exception("Unknown message");
        }
    }

    private void HandleRunStates(RunStateMessage info) {
        CanStart = !info.IsRunning;
        CanResume = info.IsPaused;
        ToastMessage = "";
        ToastBackground = "Transparent";
        if (info.IsRunning) {
            ToastMessage = "script is running";
            ToastBackground = "DarkGreen";
        }

        if (info.IsPaused) {
            ToastMessage = "script is paused";
            ToastBackground = "Orange";
        }
    }

    public TabItemViewModel ScriptViewModel { get; set; }
    public ICommand ResumeCommand { get; }
    public ICommand SaveFileCommand { get; }
    public ICommand OpenFileCommand { get; }
    public ICommand ForceCloseCommand { get; }
    public ICommand StartCommand { get; }
    public ICommand InstallBrowserCommand { get; }
    public ICommand RunPerfCommand { get; }
    public ICommand StopCommand { get; }

    public ICommand ScreenshotCommand { get; }

    public string Hello {
        get => _hello;
        set => this.RaiseAndSetIfChanged(ref _hello, value);
    }

    public ICommand ClearTracesCommand { get; }
    public ICommand SaveTraceCommand { get; }

    public string MyFortuneCookie {
        get => _myFortuneCookie;
        set => this.RaiseAndSetIfChanged(ref _myFortuneCookie, value);
    }

    public bool CanStart {
        get => _canStart;
        private set => this.RaiseAndSetIfChanged(ref _canStart, value);
    }

    public bool CanResume {
        get => _canResume;
        private set => this.RaiseAndSetIfChanged(ref _canResume, value);
    }

    public string ToastBackground {
        get => _toastBackground;
        private set => this.RaiseAndSetIfChanged(ref _toastBackground, value);
    }

    public object ToastMessage {
        get => _toastMessage;
        private set => this.RaiseAndSetIfChanged(ref _toastMessage, value);
    }

    private void OnClearTraces() {
        var lifetime = (IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime;
        var tr = lifetime.MainWindow.FindControl<ReqRespTraceControl>("TraceControl");
        tr.ViewModel.ClearTracesCommand.Execute(null);
    }

    private void OnSaveTrace() {
        var lifetime = (IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime;
        var tr = lifetime.MainWindow.FindControl<ReqRespTraceControl>("TraceControl");
        tr.ViewModel.SaveTraceCommand.Execute(null);
    }


    private async Task OnStart() {
        await OnForceClose();
        CanStart = false;

        _runner = new ScriptRunner(ScriptViewModel.Model);
        await _runner.Run();
    }

    private async Task OnStop() {
        _runner?.Stop();
    }

    private async Task OnForceClose() {
        CanStart = true;
        if (_runner != null) await _runner.ForceClose();
    }

    private async Task OnInstallBrowser() {
        Console.WriteLine("Installing Browsers for playwright. Please wait...");

        var done = false;

        var printTask = Task.Run(async () => {
            while (!done) {
                Console.Write(".");
                await Task.Delay(1000);
            }
        });

        var downloadTask = Task.Run(() => {
            var exitCode = Microsoft.Playwright.Program.Main(new[] { "install" });
            done = true;
            Console.WriteLine();
            if (exitCode != 0)
                Console.WriteLine($"Browser installation failed. Playwright exited with code {exitCode}");
            else
                Console.WriteLine($"Browser installation was successful :)");
        });

        await Task.WhenAll(downloadTask, printTask);
    }

    private async Task OnOpenFile() {
        var dg = new OpenFileDialog();
        dg.AllowMultiple = false;
        dg.Title = "Select a script file (*.csx)";
        dg.Filters = new List<FileDialogFilter>() {
            new() { Extensions = new List<string>() { "csx" }, Name = "csx" }
        };

        var lifetime = (IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime;
        var window = lifetime.MainWindow;

        var files = await dg.ShowAsync(window);
        if ((bool)files?.Any()) {
            ScriptViewModel.Model.FromFile(files[0]);
            Hello = Path.GetFileName(files[0]);
            MyFortuneCookie = files[0];
        }
    }

    private void OnResume() {
        CanResume = false;
        _runner.Resume();
    }

    private async Task OnRunPerf() {
        var w = new PerfWindow(ScriptViewModel.Model);
        w.Show();
    }

    private async Task OnScreenshot() {
        var ctx = _runner.RunContext;
        var page = ctx.TestEnv.CurrentPage;
        await ScriptingExtensions.ScriptFunctions.Screenshot(page);
    }

    private async Task OnSave() {
        var model = ScriptViewModel.Model;

        if (!string.IsNullOrWhiteSpace(model.ScriptFile)) {
            if (File.Exists(model.ScriptFile)) {
                await File.WriteAllTextAsync(model.ScriptFile, model.ScriptContent);
                Hello = Hello.TrimEnd('*');
            }
        }
        else {
            var lifetime = (IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime;
            var window = lifetime.MainWindow;
            var dg = new SaveFileDialog {
                DefaultExtension = ".csx",
                InitialFileName = "script.csx"
            };
            var file = await dg.ShowAsync(window);
            if (!string.IsNullOrWhiteSpace(file)) {
                await File.WriteAllTextAsync(file, model.ScriptContent);
                ScriptViewModel.Model.FromFile(file);
                Hello = Path.GetFileName(file);
                MyFortuneCookie = file;
            }
        }
    }
}
