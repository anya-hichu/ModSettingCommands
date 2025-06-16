using ModSettingCommands.Cmds.Msc.Options.Tmp;

namespace ModSettingCommands.Cmds.Msc.Tmp;

public class TmpSetCmd : SetCmd
{
    public TmpSetCmd(TmpSetCmdOptions options) : base(options)
    {
        AddOption(options.Key);
        AddOption(options.Source);
    }
}
