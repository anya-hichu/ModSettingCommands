namespace ModSettingCommands.Cmds.Msc.Args.Tmp;

public record TmpSetCmdArgs : SetCmdArgs
{
    public int? Key { get; set; }
    public string Source { get; set; } = default!;
}
