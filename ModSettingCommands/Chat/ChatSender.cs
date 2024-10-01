using Dalamud.Plugin.Services;
using System;
using System.Collections.Generic;

namespace ModSettingCommands.Chat;

public class ChatSender : IDisposable
{
    public ChatServer Chat { get; init; }
    public IFramework Framework { get; init; }

    public Queue<string> PendingMessages { get; init; } = [];

    public ChatSender(ChatServer chat, IFramework framework)
    {
        Chat = chat;
        Framework = framework;

        Framework.Update += OnFrameworkUpdate;
    }

    public void Dispose()
    {
        Framework.Update -= OnFrameworkUpdate;
    }

    private void OnFrameworkUpdate(IFramework framework)
    {
        while (PendingMessages.TryDequeue(out var message))
        {
            Chat.SendMessage(message);
        }
    }
    public void Enqueue(string message)
    {
        PendingMessages.Enqueue(message);
    }
}
