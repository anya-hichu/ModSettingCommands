namespace ModSettingCommands.Cmds.Msc.Args.Tmp;

public record TmpAddCmdArgs : AddCmdArgs
{
    public int? Key { get; set; }
    public string Source { get; set; } = default!;
}
