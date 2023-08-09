using System.Drawing;
using System.Threading.Tasks;
using Microsoft.Playwright;
using PlaywrightTest.Core;

namespace PlaywrightTest.ScriptingExtensions;

public static class Assert {
    public static async Task<bool> Exists(ILocator locator, string title, string group="", string desc="") {
        try {
            await Assertions
                .Expect(locator)
                .ToHaveCountAsync(1, new LocatorAssertionsToHaveCountOptions() { Timeout = 100 });

            MessageHub.Publish(AssertResult.From(title, true, group, desc));
        }
        catch (PlaywrightException pex) {
            MessageHub.Publish(AssertResult.From(title, pex, group, desc));
        }

        return true;
    }
}
