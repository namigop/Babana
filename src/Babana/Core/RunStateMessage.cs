namespace PlaywrightTest.Core;

public class RunStateMessage : IMessageContent {
    public bool IsRunning { get; set; }
    public bool IsPaused { get; set; }
    public bool IsFinished { get; set; }
}

public class PerfStatusMessage : IMessageContent{
    public bool IsRunning { get; set; }
}
