using System.Collections.Generic;
using System.CommandLine;

namespace ModSettingCommands.Cmds.Msc.Options;

public record AddCmdOptions(
    ModCmdOptions modOptions,
    Option<string> Group,
    Option<List<string>> Options) : ModCmdOptions(modOptions.Collection, modOptions.ModDir, modOptions.ModName);
