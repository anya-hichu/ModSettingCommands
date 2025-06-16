using ModSettingCommands.Cmds.Msc.Options;

namespace ModSettingCommands.Cmds.Msc;

public class AssertCmd : ModCmd
{
    public AssertCmd(AssertCmdOptions options) : base(options, "assert", "Assert group options")
    {
        AddOption(options.Group);
        AddOption(options.Options);
        AddOption(options.Enabled);
        AddOption(options.Priority);
        AddOption(options.SuccessCmds);
        AddOption(options.FailureCmds);
    }
}
