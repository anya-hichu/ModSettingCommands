using Dalamud.Plugin.Services;
using System.CommandLine.IO;
using ICommandLineConsole = System.CommandLine.IConsole;

namespace ModSettingCommands.Utils;

public class ChatGuiConsole(IChatGui chatGui) : ICommandLineConsole
{
    public IChatGui ChatGui { get; init; } = chatGui;

    public IStandardStreamWriter Out => new CustomStreamWriter((string text) => ChatGui.Print(text));

    public IStandardStreamWriter Error => new CustomStreamWriter((string text) => ChatGui.PrintError(text));

    public bool IsOutputRedirected => false;
    public bool IsErrorRedirected => false;
    public bool IsInputRedirected => false;
}
