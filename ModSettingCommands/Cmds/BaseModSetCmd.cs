using Dalamud.Game.Command;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Lumina.Extensions;
using Penumbra.Api.IpcSubscribers;
using System;

namespace ModSettingCommands.Cmds;

public abstract class BaseModSetCmd: IDisposable
{
    protected string Command { init; get; }
    protected string CommandHelpMessage { get; init; }

    protected IChatGui ChatGui { get; init; }
    protected ICommandManager CommandManager { get; init; }
    protected GetCollectionsByIdentifier GetCollectionsByIdentifier { get; init; }

    public BaseModSetCmd(string command, string commandHelpMessage, IChatGui chatGui, ICommandManager commandManager, IDalamudPluginInterface pluginInterface)
    {
        Command = command;
        CommandHelpMessage = commandHelpMessage;
        ChatGui = chatGui;
        CommandManager = commandManager;
        
        GetCollectionsByIdentifier = new(pluginInterface);
        CommandManager.AddHandler(Command, new CommandInfo(Handle)
        {
            HelpMessage = CommandHelpMessage
        });
    }

    public virtual void Dispose()
    {
        CommandManager.RemoveHandler(Command);
    }

    protected abstract void Handle(string command, string args);

    protected bool TryGetCollectionId(string identifier, out Guid collectionId)
    {
        var collections = GetCollectionsByIdentifier.Invoke(identifier);
        if(!collections.TryGetFirst(out var collection))
        {
            ChatGui.PrintError($"Could not find collection '{identifier}'");
            collectionId = default;
            return false;
        }
        collectionId = collection.Id;
        return true;
    }
}
