using ModSettingCommands.Cmds.Msc.Options;

namespace ModSettingCommands.Cmds.Msc;

public class RemoveCmd : ModCmd
{
    public RemoveCmd(RemoveCmdOptions options) : base(options, "remove", "Remove group options")
    {
        AddOption(options.Group);
        AddOption(options.Options);
    }
}
