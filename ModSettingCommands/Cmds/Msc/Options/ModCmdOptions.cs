using System.CommandLine;

namespace ModSettingCommands.Cmds.Msc.Options;

public record ModCmdOptions(Option<string> Collection, Option<string> ModDir, Option<string?> ModName);
