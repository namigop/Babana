using System;
using System.Threading;
using System.Threading.Tasks;
using PlaywrightTest.Core;
using PlaywrightTest.ScriptingExtensions;

namespace PlaywrightTest.Models;

public class ScriptRunContext {
    private DateTime startTime;
    private CancellationTokenSource tcs;
    private TaskCompletionSource pauseCs;
    public TimeSpan Elapsed { get; private set; }
    public TestEnvironment TestEnv { get; private set; }
    public Cancel CancelToken { get; private set; }

    public void Start() {
        TestEnv = new TestEnvironment();
        startTime = DateTime.Now;
        tcs = new CancellationTokenSource();
        CancelToken = new Cancel(tcs.Token);
        pauseCs = new TaskCompletionSource();
    }

    public void Stop() {
        Elapsed = DateTime.Now - startTime;
        tcs.Cancel();
    }

    //this method is available to the script
    public async Task Pause() {
        MessageHub.Publish(new Message() { Content = new RunStateMessage() { IsPaused = true } });
        await pauseCs.Task;
    }

    public static ScriptSetup Setup() {
        return new ScriptSetup();
    }

    public void Unpause() {
        pauseCs.SetResult();
        pauseCs = new TaskCompletionSource();
    }

    public async Task ForceClose() {
        await TestEnv.Teardown();
    }

    public void PrintOrderInfo() {
        var order = TestEnv.TestOrder;
        Console.WriteLine($"");
        Console.WriteLine($"{nameof(order.PhoneNumber)} : {order.PhoneNumber}");
        Console.WriteLine($"{nameof(order.Email)} : {order.Email}");

        Console.WriteLine($"{nameof(order.OrderRef)} : {order.OrderRef}");
        Console.WriteLine($"{nameof(order.ShipmentReference)} : {order.ShipmentReference}");
        Console.WriteLine($"{nameof(order.Iccid)} : {order.Iccid}");
        Console.WriteLine($"{nameof(order.IdentificationNumber)} : {order.IdentificationNumber}");
        Console.WriteLine($"{nameof(order.Iccid)} : {order.Iccid}");
        Console.WriteLine($"{nameof(order.RefNum)} : {order.RefNum}");
        Console.WriteLine($"Elapsed : {Elapsed.TotalSeconds} sec");
    }
}
