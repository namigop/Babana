using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Playwright;
using PlaywrightTest.Core;
using PlaywrightTest.Core.Platform;
using PlaywrightTest.ScriptingExtensions;

namespace PlaywrightTest.Models;

public class TestEnvironment {
    private IBrowser _browser;
    private IBrowserContext _context;
    private IPage _page;
    private IPlaywright _playwright;

    public TestEnvironment() {
        ReqRespTracer.Instance.Value.Traced += OnTraced;
        TestOrder = new TestEnvOrder();
    }

    public TestEnvOrder TestOrder { get; private set; }


    private void OnTraced(object? sender, ReqRespTraceData e) {
        if (e.RequestUri.EndsWith("/manage_order")) {
            var resp = Util.Deserialize<ManageOrderResponse>(e.ResponseBody);
            if (resp.Success && resp.Result.Any()) {
                TestOrder.IdentificationNumber = resp.Result[0].OrderUserDetail.IdentificationNumber;
                TestOrder.PhoneNumber = resp.Result[0].SelectedNumber;
                TestOrder.OrderRef = resp.Result[0].OrderRef;
            }
        }
    }

    private void OnRequest(object? sender, IRequest req) {
        // if (req.Url.Contains("qpkvc-app"))
        //     Console.WriteLine($">> {req.Method} {req.Url}");
    }

    private async void OnResponse(object? sender, IResponse resp) {
        if (resp.Headers.TryGetValue("content-type", out var contentType)) {
            var isJson = contentType.ToLower().Contains("/json") || contentType.ToLower().Contains("+json");
            var isText = contentType.ToLower().Contains("/text") || contentType.ToLower().Contains("+text");
            var isHtml = contentType.ToLower().Contains("/html");
            var canTrace = isHtml || isJson || isText;
            if (!canTrace)
                return;
        }

        var uri = new Uri(resp.Request.Url);
        var reqMethod = resp.Request.Method;
        var reqBody = resp.Request.PostData ?? "";

        var respBody = "";
        try {
            respBody = resp.Status is >= 300 and < 400 ? "" : await resp.TextAsync();
        }
        catch (Exception exc) {
            respBody = "";
        }

        var elapsed = Convert.ToInt64(resp.Request.Timing.ResponseEnd);
        ReqRespTracer.Trace(uri, reqMethod, reqBody, respBody, resp.Request.Headers, resp.Headers, resp.Status, elapsed);

        if (resp.Status is >= 400 and < 600) {
            await Task.Delay(1500); //wait 1 sec before taking a screenshot of the page to give it time to render
            await ScriptFunctions.Screenshot(_page);
        }
    }

    public IPage CurrentPage => _page;

    public async Task Teardown() {
        if (_context != null) {
            await _context.DisposeAsync();
            _context = null;
        }

        if (_browser != null) {
            await _browser.DisposeAsync();
            _browser = null;
        }

        _playwright?.Dispose();
        _playwright = null;
    }

    public async Task<IPage> Setup(ScriptSetup setup) {
        var opts = new BrowserTypeLaunchOptions {
            Headless = setup.IsHeadless,
            SlowMo = setup.SlomoMsec
        };

        _playwright = await Playwright.CreateAsync();

        if (setup.Browser == ScriptSetup.EDGE)
            _browser = await _playwright.Chromium.LaunchAsync(opts);
        else if (setup.Browser == ScriptSetup.FIREFOX)
            _browser = await _playwright.Firefox.LaunchAsync(opts);
        else
            _browser = await _playwright.Chromium.LaunchAsync(opts);

        _context = await _browser.NewContextAsync(new BrowserNewContextOptions {
            ViewportSize = ViewportSize.NoViewport
        });


        //_context = await _browser.NewContextAsync();


        _page = await _browser.NewPageAsync();
        _page.Request += OnRequest;
        _page.Response += OnResponse;
        _page.SetViewportSizeAsync(setup.BrowserWidth2, setup.BrowserHeight2);

        return _page;
    }
}
