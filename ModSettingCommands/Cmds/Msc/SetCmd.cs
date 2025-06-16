using ModSettingCommands.Cmds.Msc.Options;

namespace ModSettingCommands.Cmds.Msc;

public class SetCmd : ModCmd
{
    public SetCmd(SetCmdOptions options) : base(options, "set", "Set group options")
    {
        AddOption(options.Group);
        AddOption(options.Options);
        AddOption(options.Inherit);
        AddOption(options.Priority);
        AddOption(options.Enabled);
    }
}
