using Dalamud.Game.Command;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Penumbra.Api.IpcSubscribers;
using System;
using System.Linq;

namespace ModSettingCommands.Commands;

public abstract class BaseModSetCommand: IDisposable
{
    protected string Command { init; get; }
    protected string CommandHelpMessage { get; init; }
    protected ICommandManager CommandManager { get; init; }

    protected GetCollections GetCollections { get; init; }


    public BaseModSetCommand(string command, string commandHelpMessage, ICommandManager commandManager, IDalamudPluginInterface pluginInterface)
    {
        Command = command;
        CommandHelpMessage = commandHelpMessage;
        CommandManager = commandManager;
        GetCollections = new(pluginInterface);

        CommandManager.AddHandler(Command, new CommandInfo(Handler)
        {
            HelpMessage = CommandHelpMessage
        });
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
            var list = GetCollections.Invoke();
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
