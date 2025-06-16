using System.CommandLine;

namespace ModSettingCommands.Cmds.Msc.Options.Tmp;

public record TmpAddCmdOptions(
    AddCmdOptions options,
    Option<int?> Key,
    Option<string> Source
    ) : AddCmdOptions(options, options.Group, options.Options);
