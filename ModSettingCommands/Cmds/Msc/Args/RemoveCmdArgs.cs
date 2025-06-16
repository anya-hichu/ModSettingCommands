using System.Collections.Generic;

namespace ModSettingCommands.Cmds.Msc.Args;

public record RemoveCmdArgs : ModCmdArgs
{
    public string Group { get; set; } = default!;
    public List<string> Options { get; set; } = [];
}
