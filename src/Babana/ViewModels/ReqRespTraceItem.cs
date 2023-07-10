using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Avalonia.Media.Imaging;
using PlaywrightTest.Core;
using PlaywrightTest.Models;
using ReactiveUI;

namespace PlaywrightTest.ViewModels;

public class ReqRespTraceItem : ViewModelBase {
    private string _docId;
    private uint _elapsedMsec;
    private bool _isVisible;
    private DateTime _lastUpdated;
    private string _requestBody;
    private ObservableCollection<Pair<string, string>> _requestHeaders;
    private string _requestMethod;
    private string _requestUri;
    private string _responseBody;
    private List<Pair<string, string>> _responseHeaders;
    private string _statusCode;
    private DateTime _timestamp;

    public ReqRespTraceItem(ReqRespTraceData dto) {
        Dto = dto;
        if (dto.Screenshot != null) {
            var bmp = new Bitmap(new MemoryStream(dto.Screenshot));
            Screenshot = bmp;
            CanShowScreenshot = true;
            PathAndQuery = dto.ScreenshotName;
        }

        _elapsedMsec = dto.ElapsedMsec;
        _timestamp = dto.Timestamp;
        _requestBody = dto.RequestBody;
        _responseBody = dto.ResponseBody;
        _requestUri = dto.RequestUri;
        _statusCode = dto.StatusCode;
        _docId = dto.DocId;
        _lastUpdated = dto.LastUpdated;
        _requestMethod = dto.RequestMethod;

        _requestHeaders = new ObservableCollection<Pair<string, string>>();

        foreach (var kvp in dto.RequestHeaders)
            _requestHeaders.Add(new Pair<string, string> { Key = kvp.Key, Value = kvp.Value });

        _responseHeaders = new List<Pair<string, string>>();
        foreach (var kvp in dto.ResponseHeaders)
            _responseHeaders.Add(new Pair<string, string> { Key = kvp.Key, Value = kvp.Value });

        if (!string.IsNullOrEmpty(RequestUri)) {
            PathAndQuery = new Uri(RequestUri).PathAndQuery;

            //for display purposes
            _requestHeaders.Insert(0, new Pair<string, string> { Key = "Request Uri", Value = _requestUri });
        }
    }

    public ReqRespTraceData Dto { get; }

    public bool IsVisible {
        get => _isVisible;
        set => this.RaiseAndSetIfChanged(ref _isVisible, value);
    }

    public Bitmap Screenshot { get; }

    public string PathAndQuery { get; }

    public uint ElapsedMsec {
        get => _elapsedMsec;
        set => this.RaiseAndSetIfChanged(ref _elapsedMsec, value);
    }


    public DateTime Timestamp {
        get => _timestamp;
        set => this.RaiseAndSetIfChanged(ref _timestamp, value);
    }

    public string RequestBody {
        get => _requestBody;
        set => this.RaiseAndSetIfChanged(ref _requestBody, value);
    }

    public string ResponseBody {
        get => _responseBody;
        set => this.RaiseAndSetIfChanged(ref _responseBody, value);
    }

    public ObservableCollection<Pair<string, string>> RequestHeaders {
        get => _requestHeaders;
        set => this.RaiseAndSetIfChanged(ref _requestHeaders, value);
    }

    public List<Pair<string, string>> ResponseHeaders {
        get => _responseHeaders;
        set => this.RaiseAndSetIfChanged(ref _responseHeaders, value);
    }

    public string RequestUri {
        get => _requestUri;
        set => this.RaiseAndSetIfChanged(ref _requestUri, value);
    }

    public string StatusCode {
        get => _statusCode;
        set => this.RaiseAndSetIfChanged(ref _statusCode, value);
    }

    public string DocId {
        get => _docId;
        set => this.RaiseAndSetIfChanged(ref _docId, value);
    }

    public DateTime LastUpdated {
        get => _lastUpdated;
        set => this.RaiseAndSetIfChanged(ref _lastUpdated, value);
    }

    public string Color {
        get {
            if (RequestMethod == "POST")
                return HttpMethodColor.Post;
            if (RequestMethod == "PUT")
                return HttpMethodColor.Put;
            if (RequestMethod == "DELETE")
                return HttpMethodColor.Delete;
            if (RequestMethod == "PATCH")
                return HttpMethodColor.Patch;

            return HttpMethodColor.Get;
        }
    }

    public string StatusColor {
        get {
            var stat = Convert.ToUInt32(StatusCode);
            if (stat is >= 200 and < 400)
                return HttpMethodColor.Post;
            if (stat is >= 400 and < 500)
                return HttpMethodColor.Put;
            if (stat is >= 500)
                return HttpMethodColor.Delete;
            return HttpMethodColor.Get;
        }
    }

    public string RequestMethod {
        get => _requestMethod;
        set => this.RaiseAndSetIfChanged(ref _requestMethod, value);
    }

    public bool CanShowScreenshot { get; }

    public string ToJson() {
        return Util.Serialize(Dto, true);
    }

    public void PrettyPrint() {
        if (string.IsNullOrWhiteSpace(RequestBody))
            RequestBody = "//No content";
        if (string.IsNullOrWhiteSpace(ResponseBody))
            ResponseBody = "//No content";

        RequestBody = Util.PrettyPrint(RequestBody);
        ResponseBody = Util.PrettyPrint(ResponseBody);
    }
}