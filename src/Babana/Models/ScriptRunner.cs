using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using PlaywrightTest.Core;
using PlaywrightTest.ScriptingExtensions;

namespace PlaywrightTest.Models;

public class ScriptRunner {
    private readonly ScriptOptions options;
    private readonly ScriptRunContext ctx;
    public ScriptTabModel Model { get; private set; }

    public ScriptRunner(ScriptTabModel model) {
        Model = model;
        options =
            ScriptOptions.Default.AddReferences(new List<Assembly>() {
                    typeof(ScriptFunctions).Assembly
                })
                .WithImports(
                    typeof(ScriptFunctions).FullName,
                    typeof(PlatformFunctions).FullName,
                    "System",
                    "System.Text",
                    "System.Threading.Tasks",
                    "System.Math",
                    "System.Linq");
        ctx = new ScriptRunContext();
    }

    public async Task Run() {
        var script = Model.ScriptContent;

        try {
            ctx.Start();
            MessageHub.Publish(new Message() { Content = new RunStateMessage() { IsRunning = true } });
            await CSharpScript.RunAsync(script, options, ctx);
        }
        catch (CompilationErrorException e) {
            var compileErrors = string.Join(Environment.NewLine, e.Diagnostics);
            Console.WriteLine(compileErrors);
        }
        catch (TimeoutException exc) {
            MessageHub.Publish(new ScriptTimeoutMessage(){ Error = exc});
            await ScriptFunctions.Screenshot(ctx.TestEnv.CurrentPage, "(timeout) screenshot");
            Console.WriteLine(exc.ToString());
        }
        catch (Exception exc) {
            Console.WriteLine(exc.ToString());
            //this.Model.ExecutionLog = exc.ToString();
        }
        finally {
            ctx.Stop(); // stops any action but will not close the browser
            ctx.PrintOrderInfo();
            Model.Elapsed = ctx.Elapsed;
            MessageHub.Publish(new Message() { Content = new RunStateMessage() { IsRunning = false } });
        }
    }

    public ScriptRunContext RunContext => ctx;

    public void Stop() {
        ctx.Stop();
    }

    public async Task ForceClose() {
        MessageHub.Publish(new Message() { Content = new RunStateMessage() { IsFinished = true } });
        await ctx.ForceClose();
    }

    public void Resume() {
        MessageHub.Publish(new Message() { Content = new RunStateMessage() { IsRunning = true, IsPaused = false } });
        ctx.Unpause();
    }
}
