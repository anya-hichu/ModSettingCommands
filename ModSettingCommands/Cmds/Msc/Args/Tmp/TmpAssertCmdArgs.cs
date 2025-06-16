namespace ModSettingCommands.Cmds.Msc.Args.Tmp;

public record TmpAssertCmdArgs : AssertCmdArgs
{
    public int? Key { get; set; }
}
