using System.Collections.Generic;
using System.CommandLine;

namespace ModSettingCommands.Cmds.Msc.Options;

public record SetCmdOptions(
    ModCmdOptions modOptions,
    Option<string?> Group,
    Option<List<string>?> Options,
    Option<bool?> Inherit,
    Option<int?> Priority,
    Option<bool?> Enabled) : ModCmdOptions(modOptions.Collection, modOptions.ModDir, modOptions.ModName);
