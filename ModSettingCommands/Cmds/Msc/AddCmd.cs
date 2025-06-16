using ModSettingCommands.Cmds.Msc.Options;

namespace ModSettingCommands.Cmds.Msc;

public class AddCmd : ModCmd
{
    public AddCmd(AddCmdOptions options) : base(options, "add", "Add group options")
    {
        AddOption(options.Group);
        AddOption(options.Options);
    }
}
