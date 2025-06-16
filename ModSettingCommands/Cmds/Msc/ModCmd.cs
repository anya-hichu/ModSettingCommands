using ModSettingCommands.Cmds.Msc.Options;
using System.CommandLine;

namespace ModSettingCommands.Cmds.Msc;

abstract public class ModCmd : Command
{
    public ModCmd(ModCmdOptions options, string name, string description) : base(name, description)
    {
        AddOption(options.Collection);
        AddOption(options.ModDir);
        AddOption(options.ModName);
    }
}
