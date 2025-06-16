using System.Collections.Generic;
using System.CommandLine;

namespace ModSettingCommands.Cmds.Msc.Options;

public record ClearCmdOptions(
    ModCmdOptions modOptions,
    Option<string> Group) : ModCmdOptions(modOptions.Collection, modOptions.ModDir, modOptions.ModName);
