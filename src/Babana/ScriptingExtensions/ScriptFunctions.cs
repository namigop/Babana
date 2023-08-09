using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia.Input;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Playwright;
using PlaywrightTest.Models;

namespace PlaywrightTest.ScriptingExtensions;

public static class ScriptFunctions {
    public static void Print(string msg) {
        Console.WriteLine(msg);
    }

    public static async Task Open(this IPage? page, string url, Cancel cancel = null) {
        cancel?.TryCancel();

        await page.GotoAsync(url, new PageGotoOptions() { Timeout = 60 * 1000 });
    }

    public static async Task Refresh(this IPage? page, Cancel cancel = null) {
        cancel?.TryCancel();

        await page.ReloadAsync();
    }

    #region Keyboard

    public static async Task Keyboard(this IPage? page, string text, Cancel cancel = null) {
        cancel?.TryCancel();
        await page.Keyboard.TypeAsync(text);
        page.Keyboard.PressAsync("Enter");
    }

    #endregion

    #region waitFor functions

    public static async Task WaitFor(this IPage? page, string pattern, Cancel cancel = null) {
        cancel?.TryCancel();
        var regex = new Regex(pattern);
        var sw = Stopwatch.StartNew();
        await page?.WaitForURLAsync(
            regex,
            new PageWaitForURLOptions() {
                WaitUntil = WaitUntilState.DOMContentLoaded ,
                Timeout = 60 * 1000
            });
        sw.Stop();

        PageTracer.Instance.Value.Trace(pattern, "document",
            new RequestTimingResult() {
                ConnectEnd = -1,
                ConnectStart = -1,
                DomainLookupStart = -1,
                SecureConnectionStart = -1,
                DomainLookupEnd = -1,
                RequestStart = -1,
                ResponseEnd = sw.ElapsedMilliseconds
            });
    }

    #endregion

    #region locator functions

    public static Task Click(this ILocator? locator, Cancel cancel = null) {
        cancel?.TryCancel();
        return locator?.ClickAsync();
    }

    public static Task Check(this ILocator? locator, Cancel cancel = null) {
        cancel?.TryCancel();

        return locator?.SetCheckedAsync(true);
    }

    public static Task Uncheck(this ILocator? locator, Cancel cancel = null) {
        cancel?.TryCancel();
        return locator?.SetCheckedAsync(false);
    }

    public static async Task MouseWheel(this IPage? page, float deltaX, float deltaY) {
        await page.Mouse.WheelAsync(deltaX, deltaY);
    }

    #endregion

    #region Sleep functions

    public static Task Sleep(int msec, Cancel cancel = null) {
        cancel?.TryCancel();
        return Task.Delay(msec);
    }

    #endregion

    #region Fill functions

    public static Task Fill(this ILocator? locator, string arg, Cancel cancel = null) {
        cancel?.TryCancel();
        return locator?.FillAsync(arg);
    }

    public static async Task FillMany(this ILocator? locator, string arg, Cancel cancel = null) {
        cancel?.TryCancel();
        var parts = arg.ToCharArray().Select(c => c.ToString()).ToArray();
        var counter = 0;
        foreach (var l in await locator?.AllAsync()) {
            await l.FillAsync(parts[counter]);
            counter++;
        }
    }

    #endregion

    #region Filter functions

    public static ILocator? FilterByText(this ILocator? locator, IPage page, string arg, Cancel cancel = null) {
        cancel?.TryCancel();
        var ret = locator?.Filter(new LocatorFilterOptions { Has = page.GetByText(arg) });
        return ret;
    }

    public static ILocator? FilterById(this ILocator? locator, IPage page, string arg, Cancel cancel = null) {
        cancel?.TryCancel();
        var ret = locator?.Filter(new LocatorFilterOptions { Has = page.GetByTestId(arg) });
        return ret;
    }

    #endregion

    #region Find functions

    public static ILocator? FindById(this IPage? page, string arg, Cancel cancel = null) {
        cancel?.TryCancel();
        var ret = page?.GetByTestId(arg);
        return ret;
    }

    public static ILocator? FindById(this ILocator? locator, string arg, Cancel cancel = null) {
        cancel?.TryCancel();
        var ret = locator?.GetByTestId(arg);
        return ret;
    }

    public static ILocator? FindById(this IFrameLocator? locator, string arg, Cancel cancel = null) {
        cancel?.TryCancel();
        var ret = locator?.GetByTestId(arg);
        return ret;
    }

    public static ILocator? FindByPlaceholder(this IPage? page, string arg, Cancel cancel = null) {
        cancel?.TryCancel();
        var ret = page?.GetByPlaceholder(arg);
        return ret;
    }

    public static ILocator? FindByPlaceholder(this ILocator? locator, string arg, Cancel cancel = null) {
        cancel?.TryCancel();
        var ret = locator?.GetByPlaceholder(arg);
        return ret;
    }

    public static ILocator? FindByPlaceholder(this IFrameLocator? locator, string arg, Cancel cancel = null) {
        cancel?.TryCancel();
        var ret = locator?.GetByPlaceholder(arg);
        return ret;
    }

    public static ILocator? FindByText(this IPage? page, string arg, Cancel cancel = null) {
        cancel?.TryCancel();
        var ret = page?.GetByText(arg);
        return ret;
    }

    public static ILocator? FindByText(this ILocator? locator, string arg, Cancel cancel = null) {
        cancel?.TryCancel();
        var ret = locator?.GetByText(arg);
        return ret;
    }

    public static ILocator? FindByText(this IFrameLocator? locator, string arg, Cancel cancel = null) {
        cancel?.TryCancel();
        var ret = locator?.GetByText(arg);
        return ret;
    }

    public static ILocator? FindCheckbox(this IPage? page, Cancel cancel = null) {
        cancel?.TryCancel();
        var ret = page?.GetByRole(AriaRole.Checkbox);
        return ret;
    }

    public static ILocator? FindCheckbox(this ILocator? locator, Cancel cancel = null) {
        cancel?.TryCancel();
        var ret = locator?.GetByRole(AriaRole.Checkbox);
        return ret;
    }

    public static ILocator? FindCheckbox(this IFrameLocator? locator, Cancel cancel = null) {
        cancel?.TryCancel();
        var ret = locator?.GetByRole(AriaRole.Checkbox);
        return ret;
    }


    public static ILocator? FindButton(this IPage? page, Cancel cancel = null) {
        cancel?.TryCancel();
        var ret = page?.GetByRole(AriaRole.Button);
        return ret;
    }

    public static ILocator? FindButton(this ILocator? locator, Cancel cancel = null) {
        cancel?.TryCancel();
        var ret = locator?.GetByRole(AriaRole.Button);
        return ret;
    }

    public static ILocator? FindButton(this IFrameLocator? locator, Cancel cancel = null) {
        cancel?.TryCancel();
        var ret = locator?.GetByRole(AriaRole.Button);
        return ret;
    }

    public static ILocator? FindListItem(this IPage? page, Cancel cancel = null) {
        cancel?.TryCancel();
        var ret = page?.GetByRole(AriaRole.Listitem);
        return ret;
    }

    public static ILocator? FindListItem(this ILocator? locator, Cancel cancel = null) {
        cancel?.TryCancel();
        var ret = locator?.GetByRole(AriaRole.Listitem);
        return ret;
    }

    public static ILocator? FindListItem(this IFrameLocator? locator, Cancel cancel = null) {
        cancel?.TryCancel();
        var ret = locator?.GetByRole(AriaRole.Listitem);
        return ret;
    }

    public static ILocator? FindTextBox(this IPage? page, Cancel cancel = null) {
        cancel?.TryCancel();
        var ret = page?.GetByRole(AriaRole.Textbox);
        return ret;
    }

    public static ILocator? FindTextBox(this ILocator? locator, Cancel cancel = null) {
        cancel?.TryCancel();
        var ret = locator?.GetByRole(AriaRole.Textbox);
        return ret;
    }

    public static ILocator? FindTextBox(this IFrameLocator? locator, Cancel cancel = null) {
        cancel?.TryCancel();
        var ret = locator?.GetByRole(AriaRole.Textbox);
        return ret;
    }

    public static IFrameLocator? FindFrame(this IPage? page, string urlFragment, Cancel cancel = null) {
        cancel?.TryCancel();
        //var ret = page?.FrameByUrl(url => url.Contains(urlFragment));
        var ret = page.FrameLocator("iFrame");
        return ret;
    }

    public static ILocator? FindByName(this IPage? page, string name, Cancel cancel = null) {
        cancel?.TryCancel();
        var ret = page?.Locator($"[name={name}]");

        return ret;
    }

    public static ILocator? FindByName(this IFrameLocator? frame, string name, Cancel cancel = null) {
        cancel?.TryCancel();
        var ret = frame?.Locator($"[name={name}]");
        return ret;
    }

    #endregion


    public static async Task Screenshot(IPage? page, string name = "screenshot") {
        if (page == null)
            return;

        var bytes = await page.ScreenshotAsync(new PageScreenshotOptions { FullPage = true });
        var dto = new ReqRespTraceData() {
            Timestamp = DateTime.Now,
            ElapsedMsec = 0,
            RequestBody = "",
            ResponseBody = "",
            LastUpdated = DateTime.Now,
            RequestUri = "",
            StatusCode = "418",
            RequestMethod = "IMG",
            RequestHeaders = new Dictionary<string, string>(),
            ResponseHeaders = new Dictionary<string, string>(),
            Screenshot = bytes,
            ScreenshotName = name
        };

        ReqRespTracer.Instance.Value.Trace(dto);
    }
}
