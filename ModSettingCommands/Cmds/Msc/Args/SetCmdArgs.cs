using System.Collections.Generic;

namespace ModSettingCommands.Cmds.Msc.Args;

public record SetCmdArgs : ModCmdArgs
{
    public string? Group { get; set; }
    public List<string>? Options { get; set; }
    public bool? Inherit { get; set; }
    public int? Priority { get; set; }
    public bool? Enabled { get; set; }
}
