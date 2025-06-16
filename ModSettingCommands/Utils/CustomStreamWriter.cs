using Dalamud.Utility;
using System;
using System.CommandLine.IO;

namespace ModSettingCommands.Utils;

public class CustomStreamWriter(Action<string> action) : IStandardStreamWriter
{
    public void Write(string? value)
    {
        if (value != null && !value.IsNullOrWhitespace())
        {
            action(value.TrimEnd());
        }
    }
}
