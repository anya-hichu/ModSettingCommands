using Dalamud.Plugin.Services;
using ModSettingCommands.Utils;
using System;
using System.Collections.Generic;

namespace ModSettingCommands.Chat;

public class ChatSender : IDisposable
{
    private ChatServer ChatServer { get; init; }
    private IFramework Framework { get; init; }
    public IPluginLog PluginLog { get; init; }
    private Queue<string> Messages { get; init; } = [];

    public ChatSender(ChatServer chatServer, IFramework framework, IPluginLog pluginLog)
    {
        ChatServer = chatServer;
        Framework = framework;
        PluginLog = pluginLog;

        Framework.Update += OnFrameworkUpdate;
    }

    public void Dispose()
    {
        Framework.Update -= OnFrameworkUpdate;
    }

    public void Enqueue(string message)
    {
        Messages.Enqueue(message);
    }

    private void OnFrameworkUpdate(IFramework framework)
    {
        while (Messages.TryDequeue(out var message))
        {
            ChatServer.SendMessage(message);
            PluginLog.Verbose($"Sent chat message: '{message}'");
        }
    }
}
