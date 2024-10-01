using Dalamud.Game.Command;
using Dalamud.Plugin;
using Dalamud.Plugin.Ipc;
using Dalamud.Plugin.Services;
using ModSettingCommands.PenumbraApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ModSettingCommands.Commands;

public abstract class BaseModSetCommand: IDisposable
{
    protected string Command { init; get; }
    protected string CommandHelpMessage { get; init; }
    protected ICommandManager CommandManager { get; init; }
    protected ICallGateSubscriber<Dictionary<Guid, string>> GetCollectionsSubscriber { get; init; }
    protected ICallGateSubscriber<Guid, string, string, bool, (PenumbraApiEc, (bool, int, Dictionary<string, List<string>>, bool)?)> GetCurrentModSettings { get; init; }

    public BaseModSetCommand(string command, string commandHelpMessage, ICommandManager commandManager, IDalamudPluginInterface pluginInterface)
    {
        Command = command;
        CommandHelpMessage = commandHelpMessage;
        CommandManager = commandManager;

        CommandManager.AddHandler(Command, new CommandInfo(Handler)
        {
            HelpMessage = CommandHelpMessage
        });

        GetCollectionsSubscriber = pluginInterface.GetIpcSubscriber<Dictionary<Guid, string>>("Penumbra.GetCollections.V5");
        GetCurrentModSettings = pluginInterface.GetIpcSubscriber<Guid, string, string, bool, (PenumbraApiEc, (bool, int, Dictionary<string, List<string>>, bool)?)>("Penumbra.GetCurrentModSettings.V5");
    }

    public void Dispose()
    {
        CommandManager.RemoveHandler(Command);
    }

    protected abstract void Handler(string command, string args);

    protected Guid ParseOrRetrieveCollectionGuid(string collectionNameOrGuid)
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

}
