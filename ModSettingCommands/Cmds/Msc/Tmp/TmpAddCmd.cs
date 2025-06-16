using ModSettingCommands.Cmds.Msc.Options.Tmp;

namespace ModSettingCommands.Cmds.Msc.Tmp;

public class TmpAddCmd : AddCmd
{
    public TmpAddCmd(TmpAddCmdOptions options) : base(options)
    {
        AddOption(options.Key);
        AddOption(options.Source);
    }
}
