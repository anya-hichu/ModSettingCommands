using Dalamud.Plugin.Services;
using Dalamud.Plugin;
using ModSettingCommands.Utils;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Threading;
using ModSettingCommands.Chat;

namespace ModSettingCommands.Commands;

public partial class IfModSetCommand(IChatGui chatGui, ChatSender chatSender, ICommandManager commandManager, IDalamudPluginInterface pluginInterface, IPluginLog pluginLog) : BaseModSetCommand(COMMAND, COMMAND_HELP_MESSAGE, commandManager, pluginInterface)
{
    private static readonly int DEFAULT_MESSAGE_INTERVAL_MS = 60;

    private static readonly string OPEN_TAG = "<";
    private static readonly string CLOSE_TAG = ">";

    [GeneratedRegexAttribute(@"(?<=^|[^[])\[(?=[^[])")]
    private static partial Regex OpenEscapedTagGeneratedRegex();

    [GeneratedRegexAttribute(@"(?<=[^]])\](?=[^]]|$)")]
    private static partial Regex CloseEscapedTagGeneratedRegex();

    [GeneratedRegexAttribute(@"<wait\.(\d+)>")]
    private static partial Regex WaitTimeGeneratedRegex();

    private static readonly string COMMAND = "/ifmodset";
    private static readonly string COMMAND_HELP_MESSAGE = $"Usage: {COMMAND} -(?|!|$|e) [Collection Name or Guid] [Mod Directory] [Mod Name]( [Setting Name] ==( [Setting Value])*)?( ;( [Command])*)?";

    private static readonly char VERBOSE_FLAG = '?';
    private static readonly char DRY_RUN_FLAG = '!';
    private static readonly char ABORT_FLAG = '$';
    private static readonly char ENABLED_FLAG = 'e';
    private static readonly HashSet<char> KNOWN_FLAGS = [DRY_RUN_FLAG, VERBOSE_FLAG, ABORT_FLAG, ENABLED_FLAG];

    private static readonly string ABORT_COMMAND = "/macrocancel";

    private IChatGui ChatGui { get; init; } = chatGui;
    private ChatSender ChatSender { get; init; } = chatSender;
    private IPluginLog PluginLog { get; init; } = pluginLog;

    protected override void Handler(string command, string args)
    {
        var parsedArgs = Arguments.SplitCommandLine(args).ToList();
        var flagArgs = parsedArgs.TakeWhile(a => a.StartsWith('-'));

        var nonFlagArgs = parsedArgs[flagArgs.Count()..];
        var separatorIndex = nonFlagArgs.IndexOf(";");

        var flags = flagArgs.SelectMany(a => a[1..].ToCharArray()).ToHashSet();
        if (flags.Except(KNOWN_FLAGS).Any())
        {
            ChatGui.PrintError(CommandHelpMessage);
            return;
        }

        var conditionArgs = separatorIndex == -1 ? nonFlagArgs : nonFlagArgs[..separatorIndex];
        var commandArgs = separatorIndex == -1 ? [] : nonFlagArgs[(separatorIndex + 1)..];

        var noSettingArgs = conditionArgs.Count == 3;
        if ((noSettingArgs || (conditionArgs.Count >= 5 && conditionArgs[4] == "==")))
        {
            var collectionNameOrGuid = conditionArgs[0];
            var modDir = conditionArgs[1];
            var modName = conditionArgs[2];

            try
            {
                var collectionGuid = ParseOrRetrieveCollectionGuid(collectionNameOrGuid);

                var output = GetCurrentModSettings.InvokeFunc(collectionGuid, modDir, modName, true);
                var outputErrorCode = output.Item1;

                if (outputErrorCode == PenumbraApiEc.Success)
                {
                    var state = output.Item2;
                    if (state != null)
                    {
                        var match = true;
                        var stateValue = state.Value;

                        if (!noSettingArgs)
                        {
                            var settingName = conditionArgs[3];
                            var settingValueOrValues = conditionArgs[5..];

                            var currentSettings = stateValue.Item3;
                            var currentSettingValues = currentSettings.GetValueOrDefault(settingName, []).ToHashSet();
                            match &= currentSettingValues.SetEquals(settingValueOrValues);
                        }

                        if (flags.Contains(ENABLED_FLAG))
                        {
                            var enabled = stateValue.Item1;
                            match &= enabled;
                        }

                        Task.Run(() =>
                        {
                            if (match)
                            {
                                var isDryRun = flags.Contains(DRY_RUN_FLAG);
                                if (flags.Contains(ABORT_FLAG))
                                {
                                    if (flags.Contains(VERBOSE_FLAG) || isDryRun)
                                    {
                                        ChatGui.Print(ABORT_COMMAND);
                                    }

                                    if (!isDryRun)
                                    {
                                        Task.WaitAny(ChatSender.SendOnFrameworkThread(ABORT_COMMAND));
                                    }
                                }

                                foreach (var commandArg in commandArgs)
                                {
                                    var unescapedCommand = UnescapePlaceholders(commandArg);
                                    var waitTimeMatch = WaitTimeGeneratedRegex().Match(unescapedCommand);
                                    var commandWithoutWait = waitTimeMatch.Success ? WaitTimeGeneratedRegex().Replace(unescapedCommand, string.Empty) : unescapedCommand;

                                    if (flags.Contains(VERBOSE_FLAG) || isDryRun)
                                    {
                                        ChatGui.Print(commandWithoutWait);
                                    }

                                    if (!isDryRun)
                                    {
                                        Task.WaitAny(ChatSender.SendOnFrameworkThread(commandWithoutWait));
                                    }

                                    if (waitTimeMatch.Success)
                                    {
                                        var waitTimeValue = waitTimeMatch.Groups[1].Value;
                                        PluginLog.Verbose($"Pausing execution #{Task.CurrentId} after '{commandWithoutWait}' for {waitTimeValue} sec(s)");
                                        Thread.Sleep(int.Parse(waitTimeValue) * 1000);
                                    }
                                    else
                                    {
                                        Thread.Sleep(DEFAULT_MESSAGE_INTERVAL_MS);
                                    }
                                }
                            }
                        });
                    }
                }
                else
                {
                    throw new ArgumentException($"Failed to retrieve mod settings ({outputErrorCode})");
                }
            }
            catch (ArgumentException e)
            {
                ChatGui.PrintError(e.Message);
            }
        }
        else
        {
            ChatGui.PrintError(CommandHelpMessage);
        }
    }

    private static string UnescapePlaceholders(string message)
    {
        return CloseEscapedTagGeneratedRegex()
            .Replace(OpenEscapedTagGeneratedRegex().Replace(message, OPEN_TAG), CLOSE_TAG)
            .Replace("[[", "[").Replace("]]", "]");
    }
}
