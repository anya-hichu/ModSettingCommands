using System.CommandLine;

namespace ModSettingCommands.Cmds.Msc.Options.Tmp;

public record TmpSetCmdOptions(
    SetCmdOptions options,
    Option<int?> Key,
    Option<string> Source
    ) : SetCmdOptions(options, options.Group, options.Options, options.Inherit, options.Priority, options.Enabled);
