using System;
using System.Security.Cryptography;
using PlaywrightTest.Core;

namespace PlaywrightTest.ScriptingExtensions;

public class AssertResult : Exception, IMessageContent {
    public string Title { get; }
    public bool Pass { get; }
    public string Group { get; }
    public string Desc { get; }

    public string Error { get; private set; }

    private AssertResult(string title, bool pass, string group = "", string desc="") {
        Title = title;
        Pass = pass;
        Group = group;
        Desc = desc;
    }

    public static AssertResult From(string title, bool pass, string group = "", string desc = "") {
        return new AssertResult(title, pass, group, desc);
    }

    public static AssertResult From(string title, Exception exc, string group = "", string desc = "") {
        return new AssertResult(title, false, group, desc) {Error = exc.ToString()};
    }

}
