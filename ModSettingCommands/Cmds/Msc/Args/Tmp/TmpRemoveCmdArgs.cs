namespace ModSettingCommands.Cmds.Msc.Args.Tmp;

public record TmpRemoveCmdArgs : RemoveCmdArgs
{
    public int? Key { get; set; }
    public string Source { get; set; } = default!;
}
