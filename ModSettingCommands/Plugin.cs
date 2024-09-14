using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Ipc;
using Dalamud.Plugin.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ModSettingCommands;

public sealed class Plugin : IDalamudPlugin
{
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
    [PluginService] internal static IChatGui ChatGui { get; private set; } = null!;
    [PluginService] internal static IPluginLog PluginLog { get; private set; } = null!;

    private const string CommandName = "/modset";
    private const string CommandHelpMessage = "Usage: /modset [Collection Name or Guid] [Mod Directory] [Mod Name] [Setting Name]( [Setting Value])*";

    ICallGateSubscriber<Dictionary<Guid, string>> GetCollectionsSubscriber { get; init; }

    ICallGateSubscriber<Guid, string, string, string, string, int> TrySetModSettingSubscriber { get; init; }
    ICallGateSubscriber<Guid, string, string, string, IReadOnlyList<string>, int> TrySetModSettingsSubscriber { get; init; }


    public Plugin()
    {
        GetCollectionsSubscriber = PluginInterface.GetIpcSubscriber<Dictionary<Guid, string>>("Penumbra.GetCollections.V5");

        TrySetModSettingSubscriber = PluginInterface.GetIpcSubscriber<Guid, string, string, string, string, int>("Penumbra.TrySetModSetting.V5");
        TrySetModSettingsSubscriber = PluginInterface.GetIpcSubscriber<Guid, string, string, string, IReadOnlyList<string>, int>("Penumbra.TrySetModSettings.V5");

        CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = CommandHelpMessage
        });

        PluginInterface.UiBuilder.OpenConfigUi += Noop;
        PluginInterface.UiBuilder.OpenMainUi += Noop;
    }

    public void Dispose()
    {
        CommandManager.RemoveHandler(CommandName);
    }

    private void OnCommand(string command, string args)
    {
        var parsedArgs = Arguments.SplitCommandLine(args);
        if (parsedArgs.Length >= 4)
        {
            var collectionNameOrGuid = parsedArgs[0];
            var modDir = parsedArgs[1];
            var modName = parsedArgs[2];
            var settingName = parsedArgs[3];
            var settingValueOrValues = parsedArgs[4..];
            try
            {
                var collectionGuid = ParseOrRetrieveCollectionGuid(collectionNameOrGuid);

                PenumbraApiEc errorCode;
                if (settingValueOrValues.Length != 1)
                {
                    errorCode = (PenumbraApiEc)TrySetModSettingsSubscriber.InvokeFunc(collectionGuid, modDir, modName, settingName, settingValueOrValues);
                } 
                else
                {
                    errorCode = (PenumbraApiEc)TrySetModSettingSubscriber.InvokeFunc(collectionGuid, modDir, modName, settingName, settingValueOrValues[0]);
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
                        if (settingValueOrValues.Length > 1)
                        {
                            ChatGui.PrintError($"Couldn't find all options with names {string.Join(", ", settingValueOrValues.Select(v => $"'{v}'"))}");
                        } 
                        else
                        {
                            ChatGui.PrintError($"Couldn't find option with name '{settingValueOrValues[0]}'");
                        }
                        break;
                    default:
                        ChatGui.PrintError($"Failed to change setting(s) with error '{errorCode}'");
                        break;
                }
            } 
            catch(ArgumentException e)
            {
                ChatGui.PrintError(e.Message);
            }
        } 
        else
        {
            ChatGui.Print(CommandHelpMessage);
        }
    }

    private Guid ParseOrRetrieveCollectionGuid(string collectionNameOrGuid)
    {
        if (Guid.TryParse(collectionNameOrGuid, out var parsedGuid))
        {
           return parsedGuid;
        }
        else
        {
            var list = GetCollectionsSubscriber.InvokeFunc();
            var guid = list.FirstOrDefault(x => x.Value == collectionNameOrGuid).Key;
            if (guid != Guid.Empty)
            {
                return guid;
            }
            else
            {
                throw new ArgumentException("Invalid collection name");
            }

        }
    }

    private static void Noop() { }
}
