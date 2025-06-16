using System.CommandLine;

namespace ModSettingCommands.Cmds.Msc.Options.Tmp;

public record TmpClearCmdOptions(
    ClearCmdOptions options,
    Option<int?> Key,
    Option<string> Source) : ClearCmdOptions(options, options.Group);
