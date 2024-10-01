using Dalamud.Plugin;
using Dalamud.Plugin.Ipc;
using Dalamud.Plugin.Services;
using ModSettingCommands.PenumbraApi;
using ModSettingCommands.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ModSettingCommands.Commands;

public class ModSetCommand(IChatGui chatGui, ICommandManager commandManager, IDalamudPluginInterface pluginInterface, IPluginLog pluginLog) : BaseModSetCommand(COMMAND, COMMAND_HELP_MESSAGE, commandManager, pluginInterface)
{
    private static readonly string COMMAND = "/modset";
    private static readonly string COMMAND_HELP_MESSAGE = $"Usage: {COMMAND} [Collection Name or Guid] [Mod Directory] [Mod Name] [Setting Name] (=|+=|-=)( [Setting Value])*";

    private IChatGui ChatGui { get; init; } = chatGui;
    private IPluginLog PluginLog { get; init; } = pluginLog;
    private ICallGateSubscriber<Guid, string, string, string, string, int> TrySetModSettingSubscriber { get; init; } = pluginInterface.GetIpcSubscriber<Guid, string, string, string, string, int>("Penumbra.TrySetModSetting.V5");
    private ICallGateSubscriber<Guid, string, string, string, IReadOnlyList<string>, int> TrySetModSettingsSubscriber { get; init; } = pluginInterface.GetIpcSubscriber<Guid, string, string, string, IReadOnlyList<string>, int>("Penumbra.TrySetModSettings.V5");

    protected override void Handler(string command, string args)
    {
        var parsedArgs = Arguments.SplitCommandLine(args);
        if (parsedArgs.Length >= 5)
        {
            var collectionNameOrGuid = parsedArgs[0];
            var modDir = parsedArgs[1];
            var modName = parsedArgs[2];
            var settingName = parsedArgs[3];
            var assignmentOperator = parsedArgs[4];
            var settingValueOrValues = parsedArgs[5..];
            try
            {
                var collectionGuid = ParseOrRetrieveCollectionGuid(collectionNameOrGuid);

                PenumbraApiEc errorCode;

                var isUnionOperator = assignmentOperator == "+=";
                var isExceptOperator = assignmentOperator == "-=";
                if (assignmentOperator == "=")
                {
                    // Stateless
                    if (settingValueOrValues.Length != 1)
                    {
                        errorCode = (PenumbraApiEc)TrySetModSettingsSubscriber.InvokeFunc(collectionGuid, modDir, modName, settingName, settingValueOrValues);
                    }
                    else
                    {
                        errorCode = (PenumbraApiEc)TrySetModSettingSubscriber.InvokeFunc(collectionGuid, modDir, modName, settingName, settingValueOrValues[0]);
                    }
                }
                else if (isUnionOperator || isExceptOperator)
                {
                    // Stateful
                    var output = GetCurrentModSettings.InvokeFunc(collectionGuid, modDir, modName, true);
                    var outputErrorCode = output.Item1;
                    if (outputErrorCode == PenumbraApiEc.Success)
                    {
                        var state = output.Item2;
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
                            errorCode = (PenumbraApiEc)TrySetModSettingsSubscriber.InvokeFunc(collectionGuid, modDir, modName, settingName, newSettings);
                        }
                        else
                        {
                            errorCode = PenumbraApiEc.NothingChanged;
                        }
                    }
                    else
                    {
                        errorCode = outputErrorCode;
                    }
                }
                else
                {
                    throw new ArgumentException($"Unsupported assignment operator '{assignmentOperator}'");
                }

                switch (errorCode)
                {
                    case PenumbraApiEc.Success:
                        PluginLog.Info($"Settings have been changed with command: {command} {args}");
                        break;
                    case PenumbraApiEc.NothingChanged:
                        PluginLog.Info($"No setting has been changed with command: {command} {args}");
                        break;
                    case PenumbraApiEc.CollectionMissing:
                        ChatGui.PrintError($"Couldn't find collection with name or guid '{collectionNameOrGuid}'");
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
                        ChatGui.PrintError($"Failed to change setting(s) with error '{errorCode}'");
                        break;
                }
            }
            catch (ArgumentException e)
            {
                ChatGui.PrintError(e.Message);
            }
        }
        else
        {
            ChatGui.Print(CommandHelpMessage);
        }
    }
}
