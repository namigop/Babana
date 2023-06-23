using System;

namespace PlaywrightTest.Core;

public static class MessageHub {
    public static event EventHandler<Message> Sub;

    public static void Publish(Message msg, object sender = null) {
        Sub?.Invoke(sender, msg);
    }
}