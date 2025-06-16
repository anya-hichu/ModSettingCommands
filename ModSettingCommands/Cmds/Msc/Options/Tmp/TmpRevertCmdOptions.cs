using System.CommandLine;

namespace ModSettingCommands.Cmds.Msc.Options.Tmp;

public record TmpRevertCmdOptions(Option<string> Collection, Option<int?> Key);
