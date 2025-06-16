using System.Collections.Generic;

namespace ModSettingCommands.Cmds.Msc.Args;

public record AssertCmdArgs : ModCmdArgs
{
    public string? Group { get; set; } = default!;
    public List<string>? Options { get; set; } = [];

    public bool? Inherit { get; set; }
    public int? Priority { get; set; }
    public bool? Enabled { get; set; }

    public List<string> SuccessCmds { get; set; } = [];
    public List<string> FailureCmds { get; set; } = [];
}
