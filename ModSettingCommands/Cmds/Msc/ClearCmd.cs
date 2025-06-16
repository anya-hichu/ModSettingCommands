using Dalamud.Plugin.Services;
using ModSettingCommands.Cmds.Msc.Options;
using System.CommandLine;

namespace ModSettingCommands.Cmds.Msc;

public class ClearCmd : Command
{
    public ClearCmd(ClearCmdOptions options) : base("clear", "Clear groups")
    {
        AddOption(options.Collection);
        AddOption(options.ModDir);
        AddOption(options.ModName);
        AddOption(options.Group);
    }
}
