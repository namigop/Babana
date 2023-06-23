using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using Microsoft.Playwright;
using PlaywrightTest.Core;
using PlaywrightTest.Models;
using ReactiveUI;

namespace PlaywrightTest.ViewModels;

public class ReqRespTraceViewModel : ViewModelBase {
    private ReqRespTraceItem _selectedTraceItem;
    private string _uriFilter = "qpkvc";
    private Stretch _imageStretch;
    private bool _isStretched;


    public ObservableCollection<ReqRespTraceItem> TraceItems { get; } = new();

    //for the designer
    public ReqRespTraceViewModel() {
        this.ClearTracesCommand = ReactiveCommand.Create(OnClear);
        this.SaveTraceCommand = ReactiveCommand.Create(OnSave);
        this.DisplayOptions = new ReqRespDisplayOptions();
        this.ToggleImageCommand = ReactiveCommand.Create(OnToggleImage);

        this.WhenAnyValue(
                t => t.DisplayOptions.CanShowGet,
                t => t.DisplayOptions.CanShowPost,
                t => t.DisplayOptions.CanShowPut,
                t => t.DisplayOptions.CanShowDelete,
                t => t.DisplayOptions.CanShowPatch,
                t => t.DisplayOptions.CanShowOptions,
                t => t.DisplayOptions.CanShowHead
                // t => t.UriFilter
            )
            .Throttle(TimeSpan.FromMilliseconds(150))
            .Subscribe(t => Load());

        this.WhenAnyValue(t => t.UriFilter)
            .Throttle(TimeSpan.FromMilliseconds(150))
            .Subscribe(t => Load());
        //.Void(d => disposable.Add(d));

        this.TraceItems.CollectionChanged += OnCollectionChanged;
        _imageStretch = Stretch.Uniform;
        _isStretched = true;
    }

    private void OnToggleImage() {
        if (_isStretched)
            this.ImageStretch = ImageStretch = Stretch.None;
        else {
            this.ImageStretch = ImageStretch = Stretch.Uniform;
        }

        _isStretched = !_isStretched;
    }


    private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) {
        this.RaisePropertyChanged(nameof(HasItems));
    }

    public ReqRespDisplayOptions DisplayOptions { get; }

    public ReqRespTraceItem SelectedTraceItem {
        get => _selectedTraceItem;
        set {
            this.RaiseAndSetIfChanged(ref _selectedTraceItem, value);
            _selectedTraceItem?.PrettyPrint();
        }
    }

    public ICommand SaveTraceCommand { get; }
    public ICommand ClearTracesCommand { get; }

    public bool HasItems => this.TraceItems.Any();

    public string UriFilter {
        get => _uriFilter;
        set => this.RaiseAndSetIfChanged(ref _uriFilter, value);
    }

    public ICommand ToggleImageCommand { get; }

    public Avalonia.Media.Stretch ImageStretch {
        get => _imageStretch;
        private set => this.RaiseAndSetIfChanged(ref _imageStretch, value);
    }


    private void OnClear() {
        this.TraceItems.Clear();
        this.SelectedTraceItem = null;
        ReqRespTracer.Instance.Value.ClearAll();
    }

    private async Task OnSave() {
        if (this.SelectedTraceItem != null) {
            var lifetime = (IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime;
            var window = lifetime.MainWindow;

            if (this.SelectedTraceItem.CanShowScreenshot) {
                var sd = new SaveFileDialog {
                    InitialFileName = "screenshot.png"
                };

                var imgFile = await sd.ShowAsync(window);
                if (!string.IsNullOrEmpty(imgFile)) {
                    this.SelectedTraceItem.Screenshot.Save(imgFile);
                }

                return;
            }

            var json = this.SelectedTraceItem.ToJson();
            var dg = new SaveFileDialog {
                DefaultExtension = ".json",
                InitialFileName = "trace.json"
            };
            var file = await dg.ShowAsync(window);
            if (!string.IsNullOrWhiteSpace(file))
                File.WriteAllText(file, json);
        }
    }

    public bool CheckIfCanShow(ReqRespTraceData i) {
        //screenshots are always okay
        if (i.Screenshot != null && i.Screenshot.Any())
            return true;

        if (!string.IsNullOrWhiteSpace(UriFilter)) {
            var ok = i.RequestUri.ToLowerInvariant().Contains(UriFilter.ToLowerInvariant());
            if (!ok)
                return false;
        }

        if (DisplayOptions.CanShowGet && HttpUtil.GetMethod(i.RequestMethod) == HttpMethod.Get) {
            return true;
        }

        if (DisplayOptions.CanShowPost && HttpUtil.GetMethod(i.RequestMethod) == HttpMethod.Post) {
            return true;
        }

        if (DisplayOptions.CanShowPut && HttpUtil.GetMethod(i.RequestMethod) == HttpMethod.Put) {
            return true;
        }

        if (DisplayOptions.CanShowDelete && HttpUtil.GetMethod(i.RequestMethod) == HttpMethod.Delete) {
            return true;
        }

        if (DisplayOptions.CanShowOptions && HttpUtil.GetMethod(i.RequestMethod) == HttpMethod.Options) {
            return true;
        }

        if (DisplayOptions.CanShowPatch && HttpUtil.GetMethod(i.RequestMethod) == HttpMethod.Patch) {
            return true;
        }

        if (DisplayOptions.CanShowHead && HttpUtil.GetMethod(i.RequestMethod) == HttpMethod.Head) {
            return true;
        }

        if (i.RequestMethod == "IMG")
            return true;


        return false;
    }

    private static object sync = new object();

    public void AddTrace(ReqRespTraceData i) {
        var item = new ReqRespTraceItem(i) { IsVisible = CheckIfCanShow(i) };
        lock (sync)
            this.TraceItems.Add(item);

        if (SelectedTraceItem == null && item.IsVisible)
            this.SelectedTraceItem = item;
    }

    public void Load() {
        //var all = ReqRespTracer.Instance.Value.GetAll().Where(CheckIfCanShow);
        lock (sync) {
            foreach (var i in TraceItems) {
                i.IsVisible = CheckIfCanShow(i.Dto);
            }
        }

        if (this.TraceItems.Any())
            this.SelectedTraceItem = this.TraceItems.First();
    }
}
