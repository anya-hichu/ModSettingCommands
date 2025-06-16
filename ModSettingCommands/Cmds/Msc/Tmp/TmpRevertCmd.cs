using ModSettingCommands.Cmds.Msc.Options.Tmp;
using System.CommandLine;

namespace ModSettingCommands.Cmds.Msc.Tmp;

public class TmpRevertCmd : Command
{
    public TmpRevertCmd(TmpRevertCmdOptions options) : base("revert", "Revert temporary settings")
    {
        AddOption(options.Collection);
        AddOption(options.Key);
    }
}
