using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Playwright;
using PlaywrightTest.Models;

namespace PlaywrightTest.ScriptingExtensions;

public class ScriptSetup {

    public const string CHROMIUM ="CHROMIUM";
    public const string FIREFOX ="FIREFOX";
    public const string EDGE ="EDGE";


    public ScriptSetup headless(bool headless) {
        this.IsHeadless = headless;
        this.BrowserHeight = 880;
        this.BrowserWidth = 1512;
        return this;
    }

    public int BrowserWidth { get; set; }

    public int BrowserHeight { get; set; }

    public ScriptSetup slomo(int slomo) {
        this.SlomoMsec = slomo;
        return this;
    }

    public ScriptSetup chromium() {
        this.Browser = CHROMIUM;
        return this;
    }
    public ScriptSetup firefox() {
        this.Browser = FIREFOX;
        return this;
    }
    public ScriptSetup edge() {
        this.Browser = EDGE;
        return this;
    }

    public string Browser { get; set; }

    public ScriptSetup browserHeight(int height) {
        this.BrowserHeight = height;
        return this;
    }
    public ScriptSetup browserWidth(int width) {
        this.BrowserWidth = width;
        return this;
    }

    public Cancel Cancel { get; private set; }

    public int SlomoMsec { get; private set; }


    public async Task<IPage> begin(TestEnvironment env) {
        return await env.Setup(this);
    }

    public bool IsHeadless { get; private set; }
}
