using System.CommandLine;

namespace ModSettingCommands.Cmds.Msc.Options.Tmp;

public record TmpAssertCmdOptions(
    AssertCmdOptions options,
    Option<int?> Key,
    Option<string> Source
    ) : AssertCmdOptions(
        options,
        options.Group,
        options.Options,
        options.Enabled,
        options.Priority,
        options.SuccessCmds,
        options.FailureCmds);
