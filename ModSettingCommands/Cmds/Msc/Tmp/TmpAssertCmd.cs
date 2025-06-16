using ModSettingCommands.Cmds.Msc.Options.Tmp;

namespace ModSettingCommands.Cmds.Msc.Tmp;

public class TmpAssertCmd : AssertCmd
{
    public TmpAssertCmd(TmpAssertCmdOptions options) : base(options)
    {
        AddOption(options.Key);
    }
}
