using ModSettingCommands.Cmds.Msc.Options.Tmp;

namespace ModSettingCommands.Cmds.Msc.Tmp;

public class TmpRemoveCmd : RemoveCmd
{
    public TmpRemoveCmd(TmpRemoveCmdOptions options) : base(options)
    {
        AddOption(options.Key);
        AddOption(options.Source);
    }
}
