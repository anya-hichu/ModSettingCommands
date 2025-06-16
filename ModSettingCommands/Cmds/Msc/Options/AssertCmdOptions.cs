using System.Collections.Generic;
using System.CommandLine;

namespace ModSettingCommands.Cmds.Msc.Options;

public record AssertCmdOptions(
    ModCmdOptions modOptions,
    Option<string?> Group,
    Option<List<string>?> Options,
    Option<bool?> Enabled,
    Option<int?> Priority,
    Option<List<string>> SuccessCmds,
    Option<List<string>> FailureCmds) : ModCmdOptions(modOptions.Collection, modOptions.ModDir, modOptions.ModName);
