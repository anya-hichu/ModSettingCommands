namespace ModSettingCommands.Cmds.Msc.Args.Tmp;

public record TmpRevertCmdArgs
{
    public string Collection { get; set; } = default!;
    public int? Key { get; set; }
}
