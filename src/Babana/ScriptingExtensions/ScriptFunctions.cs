using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia.Input;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Playwright;
using PlaywrightTest.Models;

namespace PlaywrightTest.ScriptingExtensions;

public static class ScriptFunctions {
    public static void print(string msg) {
        Console.WriteLine(msg);
    }

    public static async Task open(this IPage? page, string url, Cancel cancel = null) {
        cancel?.TryCancel();
        await page.GotoAsync(url, new PageGotoOptions() { Timeout = 60 * 1000 });
    }

    public static async Task refresh(this IPage? page, Cancel cancel = null) {
        cancel?.TryCancel();
        await page.ReloadAsync();
    }

    #region Keyboard

    public static async Task keyboard(this IPage? page, string text, Cancel cancel = null) {
        cancel?.TryCancel();
        await page.Keyboard.TypeAsync(text);
        page.Keyboard.PressAsync("Enter");
    }

    #endregion

    #region waitFor functions

    public static async Task waitFor(this IPage? page, string pattern, Cancel cancel = null) {
        cancel?.TryCancel();
        var regex = new Regex(pattern);
        await page?.WaitForURLAsync(regex, new PageWaitForURLOptions() { Timeout = 60 * 1000 });
    }

    #endregion

    #region locator functions

    public static Task click(this ILocator? locator, Cancel cancel = null) {
        cancel?.TryCancel();
        return locator?.ClickAsync();
    }

    public static Task check(this ILocator? locator, Cancel cancel = null) {
        cancel?.TryCancel();
        return locator?.SetCheckedAsync(true);
    }
    public static Task uncheck(this ILocator? locator, Cancel cancel = null) {
        cancel?.TryCancel();
        return locator?.SetCheckedAsync(false);

    }

    public static async Task MouseWheel(this IPage? page, float deltaX, float deltaY ) {
        await page.Mouse.WheelAsync(deltaX, deltaY);
    }


    #endregion

    #region Sleep functions

    public static Task sleep(int msec, Cancel cancel = null) {
        cancel?.TryCancel();
        return Task.Delay(msec);
    }

    #endregion

    #region Fill functions

    public static Task fill(this ILocator? locator, string arg, Cancel cancel = null) {
        cancel?.TryCancel();
        return locator?.FillAsync(arg);
    }

    public static async Task fillMany(this ILocator? locator, string arg, Cancel cancel = null) {
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

    public static ILocator? filterByText(this ILocator? locator, IPage page, string arg, Cancel cancel = null) {
        cancel?.TryCancel();
        var ret = locator?.Filter(new LocatorFilterOptions { Has = page.GetByText(arg) });
        return ret;
    }

    public static ILocator? filterById(this ILocator? locator, IPage page, string arg, Cancel cancel = null) {
        cancel?.TryCancel();
        var ret = locator?.Filter(new LocatorFilterOptions { Has = page.GetByTestId(arg) });
        return ret;
    }

    #endregion

    #region Find functions

    public static ILocator? findById(this IPage? page, string arg, Cancel cancel = null) {
        cancel?.TryCancel();
        var ret = page?.GetByTestId(arg);
        return ret;
    }

    public static ILocator? findById(this ILocator? locator, string arg, Cancel cancel = null) {
        cancel?.TryCancel();
        var ret = locator?.GetByTestId(arg);
        return ret;
    }
    public static ILocator? findById(this IFrameLocator? locator, string arg, Cancel cancel = null) {
        cancel?.TryCancel();
        var ret = locator?.GetByTestId(arg);
        return ret;
    }

    public static ILocator? findByPlaceholder(this IPage? page, string arg, Cancel cancel = null) {
        cancel?.TryCancel();
        var ret = page?.GetByPlaceholder(arg);
        return ret;
    }

    public static ILocator? findByPlaceholder(this ILocator? locator, string arg, Cancel cancel = null) {
        cancel?.TryCancel();
        var ret = locator?.GetByPlaceholder(arg);
        return ret;
    }

    public static ILocator? findByPlaceholder(this IFrameLocator? locator, string arg, Cancel cancel = null) {
        cancel?.TryCancel();
        var ret = locator?.GetByPlaceholder(arg);
        return ret;
    }

    public static ILocator? findByText(this IPage? page, string arg, Cancel cancel = null) {
        cancel?.TryCancel();
        var ret = page?.GetByText(arg);
        return ret;
    }

    public static ILocator? findByText(this ILocator? locator, string arg, Cancel cancel = null) {
        cancel?.TryCancel();
        var ret = locator?.GetByText(arg);
        return ret;
    }
    public static ILocator? findByText(this IFrameLocator? locator, string arg, Cancel cancel = null) {
        cancel?.TryCancel();
        var ret = locator?.GetByText(arg);
        return ret;
    }

    public static ILocator? findCheckbox(this IPage? page, Cancel cancel = null) {
        cancel?.TryCancel();
        var ret = page?.GetByRole(AriaRole.Checkbox);
        return ret;
    }

    public static ILocator? findCheckbox(this ILocator? locator, Cancel cancel = null) {
        cancel?.TryCancel();
        var ret = locator?.GetByRole(AriaRole.Checkbox);
        return ret;
    }

    public static ILocator? findCheckbox(this IFrameLocator? locator, Cancel cancel = null) {
        cancel?.TryCancel();
        var ret = locator?.GetByRole(AriaRole.Checkbox);
        return ret;
    }


    public static ILocator? findButton(this IPage? page, Cancel cancel = null) {
        cancel?.TryCancel();
        var ret = page?.GetByRole(AriaRole.Button);
        return ret;
    }

    public static ILocator? findButton(this ILocator? locator, Cancel cancel = null) {
        cancel?.TryCancel();
        var ret = locator?.GetByRole(AriaRole.Button);
        return ret;
    }

    public static ILocator? findButton(this IFrameLocator? locator, Cancel cancel = null) {
        cancel?.TryCancel();
        var ret = locator?.GetByRole(AriaRole.Button);
        return ret;
    }

    public static ILocator? findListItem(this IPage? page, Cancel cancel = null) {
        cancel?.TryCancel();
        var ret = page?.GetByRole(AriaRole.Listitem);
        return ret;
    }

    public static ILocator? findListItem(this ILocator? locator, Cancel cancel = null) {
        cancel?.TryCancel();
        var ret = locator?.GetByRole(AriaRole.Listitem);
        return ret;
    }

    public static ILocator? findListItem(this IFrameLocator? locator, Cancel cancel = null) {
        cancel?.TryCancel();
        var ret = locator?.GetByRole(AriaRole.Listitem);
        return ret;
    }

    public static ILocator? findTextBox(this IPage? page, Cancel cancel = null) {
        cancel?.TryCancel();
        var ret = page?.GetByRole(AriaRole.Textbox);
        return ret;
    }

    public static ILocator? findTextBox(this ILocator? locator, Cancel cancel = null) {
        cancel?.TryCancel();
        var ret = locator?.GetByRole(AriaRole.Textbox);
        return ret;
    }

    public static ILocator? findTextBox(this IFrameLocator? locator, Cancel cancel = null) {
        cancel?.TryCancel();
        var ret = locator?.GetByRole(AriaRole.Textbox);
        return ret;
    }

    public static IFrame? findFrame(this IPage? page, string urlFragment, Cancel cancel = null) {
        cancel?.TryCancel();
        var ret = page?.FrameByUrl(url => url.Contains(urlFragment));

        return ret;
    }

    public static ILocator? findByName(this IPage? page, string name, Cancel cancel = null) {
        cancel?.TryCancel();
        var ret = page?.Locator($"[name={name}]");

        return ret;
    }
    public static ILocator? findByName(this IFrameLocator? frame, string name, Cancel cancel = null) {
        cancel?.TryCancel();
        var ret = frame?.Locator($"[name={name}]");
        return ret;
    }

    #endregion


    public static async Task screenshot(IPage? page, string name = "screenshot") {
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
