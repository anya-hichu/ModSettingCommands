namespace ModSettingCommands.Cmds.Msc.Args;

public record ClearCmdArgs : ModCmdArgs
{
    public string Group { get; set; } = default!;
}
