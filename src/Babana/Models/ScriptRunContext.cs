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
        this.TestEnv = new TestEnvironment();
        this.startTime = DateTime.Now;
        this.tcs = new CancellationTokenSource();
        this.CancelToken = new Cancel(this.tcs.Token);
        this.pauseCs = new TaskCompletionSource();
    }

    public void Stop() {
        this.Elapsed = DateTime.Now - startTime;
        this.tcs.Cancel();
    }

    //this method is available to the script
    public async Task pause() {
        MessageHub.Publish(new Message(){ Content = new RunStateMessage() { IsPaused = true} });
        await this.pauseCs.Task;

    }

    public static ScriptSetup setup() {
        return new ScriptSetup();
    }

    public void Unpause() {
        this.pauseCs.SetResult();
        this.pauseCs = new TaskCompletionSource();
    }

    public async Task ForceClose() {
        await this.TestEnv.Teardown();
    }

    public void PrintOrderInfo() {
        var order = this.TestEnv.TestOrder;
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
