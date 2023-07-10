using System.Threading.Tasks;
using Microsoft.Playwright;
using PlaywrightTest.Models;

namespace PlaywrightTest.ScriptingExtensions;

public class ScriptSetup {
    public const string CHROMIUM = "CHROMIUM";
    public const string FIREFOX = "FIREFOX";
    public const string EDGE = "EDGE";


    public ScriptSetup Headless(bool headless) {
        IsHeadless = headless;
        BrowserHeight2 = 880;
        BrowserWidth2 = 1512;
        return this;
    }

    public int BrowserWidth2 { get; set; }

    public int BrowserHeight2 { get; set; }

    public ScriptSetup Slomo(int slomo) {
        SlomoMsec = slomo;
        return this;
    }

    public ScriptSetup Chromium() {
        Browser = CHROMIUM;
        return this;
    }

    public ScriptSetup Firefox() {
        Browser = FIREFOX;
        return this;
    }

    public ScriptSetup Edge() {
        Browser = EDGE;
        return this;
    }

    public string Browser { get; set; }

    public ScriptSetup BrowserHeight(int height) {
        BrowserHeight2 = height;
        return this;
    }

    public ScriptSetup BrowserWidth(int width) {
        BrowserWidth2 = width;
        return this;
    }

    public Cancel Cancel { get; private set; }

    public int SlomoMsec { get; private set; }


    public async Task<IPage> Begin(TestEnvironment env) {
        return await env.Setup(this);
    }

    public bool IsHeadless { get; private set; }
}