namespace ModSettingCommands.Cmds.Msc.Args.Tmp;

public record TmpClearCmdArgs : ClearCmdArgs
{
    public int? Key { get; set; }
    public string Source { get; set; } = default!;
}
