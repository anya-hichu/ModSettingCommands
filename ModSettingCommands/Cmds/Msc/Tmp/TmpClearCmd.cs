using ModSettingCommands.Cmds.Msc.Options.Tmp;

namespace ModSettingCommands.Cmds.Msc.Tmp;

public class TmpClearCmd : ClearCmd
{
    public TmpClearCmd(TmpClearCmdOptions options) : base(options)
    {
        AddOption(options.Key);
        AddOption(options.Source);
    }
}
