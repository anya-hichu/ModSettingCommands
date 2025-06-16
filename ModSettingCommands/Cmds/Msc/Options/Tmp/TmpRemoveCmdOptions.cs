using System.CommandLine;

namespace ModSettingCommands.Cmds.Msc.Options.Tmp;

public record TmpRemoveCmdOptions(
    RemoveCmdOptions options,
    Option<int?> Key,
    Option<string> Source) : RemoveCmdOptions(options, options.Group, options.Options);
