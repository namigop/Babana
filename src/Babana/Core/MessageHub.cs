using System;

namespace PlaywrightTest.Core;

public static class MessageHub {
    public static event EventHandler<Message> Sub;

    public static void Publish(Message msg, object sender = null) {
        Sub?.Invoke(sender, msg);
    }
    public static void Publish(IMessageContent content, object sender = null) {
        var msg = new Message(){ Content = content};
        Publish(msg, sender);
    }
}
