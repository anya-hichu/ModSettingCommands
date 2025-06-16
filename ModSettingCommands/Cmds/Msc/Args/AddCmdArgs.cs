using System.Collections.Generic;

namespace ModSettingCommands.Cmds.Msc.Args;

public record AddCmdArgs : ModCmdArgs
{
    public string Group { get; set; } = default!;
    public List<string> Options { get; set; } = [];
}
