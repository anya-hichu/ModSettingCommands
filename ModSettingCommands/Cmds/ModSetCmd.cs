using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using ModSettingCommands.Utils;
using Penumbra.Api.Enums;
using Penumbra.Api.IpcSubscribers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ModSettingCommands.Cmds;

[Obsolete($"IfModSetCmd command superseeded by MscCommand")]
public class ModSetCmd(
    IChatGui chatGui, 
    ICommandManager commandManager, 
    IDalamudPluginInterface pluginInterface, 
    IPluginLog pluginLog) : BaseModSetCmd(COMMAND, COMMAND_HELP_MESSAGE, chatGui, commandManager, pluginInterface)
{
    private static readonly string COMMAND = "/modset";
    private static readonly string COMMAND_HELP_MESSAGE = $"Usage: {COMMAND} [Collection Name or Guid] [Mod Directory] [Mod Name] [Setting Name] (=|+=|-=)( [Setting Value])*";

    private IPluginLog PluginLog { get; init; } = pluginLog;
    private TrySetModSetting TrySetModSetting { get; init; } = new(pluginInterface);
    private TrySetModSettings TrySetModSettings { get; init; } = new(pluginInterface);
    private GetCurrentModSettings GetCurrentModSettings { get; init; } = new(pluginInterface);

    protected override void Handle(string command, string args)
    {
        var parsedArgs = Arguments.SplitCommandLine(args);
        if (parsedArgs.Length >= 5)
        {
            var collectionIdentifier = parsedArgs[0];
            var modDir = parsedArgs[1];
            var modName = parsedArgs[2];
            var settingName = parsedArgs[3];
            var assignmentOperator = parsedArgs[4];
            var settingValueOrValues = parsedArgs[5..];

            if (!TryGetCollectionId(collectionIdentifier, out var collectionId))
            {
                return;
            }

            PenumbraApiEc exitCode;

            var isUnionOperator = assignmentOperator == "+=";
            var isExceptOperator = assignmentOperator == "-=";
            if (assignmentOperator == "=")
            {
                // Stateless
                if (settingValueOrValues.Length != 1)
                {
                    exitCode = TrySetModSettings.Invoke(collectionId, modDir, settingName, settingValueOrValues.AsReadOnly(), modName);
                }
                else
                {
                    exitCode = TrySetModSetting.Invoke(collectionId, modDir, settingName, settingValueOrValues[0], modName);
                }
            }
            else if (isUnionOperator || isExceptOperator)
            {
                // Stateful
                var (outputExitCode, state) = GetCurrentModSettings.Invoke(collectionId, modDir, modName, true);
                if (outputExitCode == PenumbraApiEc.Success)
                {
                    if (state != null)
                    {
                        var currentSettings = state.Value.Item3;
                        var currentSettingValues = currentSettings.GetValueOrDefault(settingName, []);

                        var newSettings = currentSettingValues;
                        if (isUnionOperator)
                        {
                            newSettings = currentSettingValues.Union(settingValueOrValues).ToList();
                        }
                        else if (isExceptOperator)
                        {
                            newSettings = currentSettingValues.Except(settingValueOrValues).ToList();
                        }
                        exitCode = TrySetModSettings.Invoke(collectionId, modDir, settingName, newSettings, modName);
                    }
                    else
                    {
                        exitCode = PenumbraApiEc.NothingChanged;
                    }
                }
                else
                {
                    exitCode = outputExitCode;
                }
            }
            else
            {
                ChatGui.PrintError($"Unsupported assignment operator '{assignmentOperator}'");
                return;
            }

            switch (exitCode)
            {
                case PenumbraApiEc.Success:
                    PluginLog.Info($"Settings have been changed with command: {command} {args}");
                    break;
                case PenumbraApiEc.NothingChanged:
                    PluginLog.Info($"No setting has been changed with command: {command} {args}");
                    break;
                case PenumbraApiEc.CollectionMissing:
                    ChatGui.PrintError($"Couldn't find collection with identifier '{collectionIdentifier}'");
                    break;
                case PenumbraApiEc.ModMissing:
                    ChatGui.PrintError($"Couldn't find mod with directory '{modDir}' and name '{modName}'");
                    break;
                case PenumbraApiEc.OptionGroupMissing:
                    ChatGui.PrintError($"Couldn't find option group with name '{settingName}'");
                    break;
                case PenumbraApiEc.OptionMissing:
                    ChatGui.PrintError($"Couldn't find all option with name(s): '{string.Join(", ", settingValueOrValues.Select(v => $"'{v}'"))}'");
                    break;
                default:
                    ChatGui.PrintError($"Failed to change setting(s) with error '{exitCode}'");
                    break;
            }
        }
        else
        {
            ChatGui.Print(CommandHelpMessage);
        }
    }
}
