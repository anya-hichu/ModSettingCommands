namespace ModSettingCommands.Cmds.Msc.Args;

public abstract record ModCmdArgs
{
    public string Collection { get; set; } = default!;
    public string ModDir { get; set; } = default!;
    public string? ModName { get; set; }
}
