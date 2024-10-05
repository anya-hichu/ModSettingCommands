using Dalamud.Plugin.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModSettingCommands.Chat;

public class ChatSender : IDisposable
{
    private class Payload(string message, TaskCompletionSource completion)
    {
        public string Message { get; init; } = message;
        public TaskCompletionSource Completion { get; init; } = completion;
    }

    private ChatServer ChatServer { get; init; }
    private IFramework Framework { get; init; }
    public IPluginLog PluginLog { get; init; }
    private Queue<Payload> PendingPayloads { get; init; } = [];

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

    public Task SendOnFrameworkThread(string message)
    {
        var completion = new TaskCompletionSource();
        PendingPayloads.Enqueue(new(message, completion));
        return completion.Task;
    }

    private void OnFrameworkUpdate(IFramework framework)
    {
        while (PendingPayloads.TryDequeue(out var payload))
        {
            var message = payload.Message;
            ChatServer.SendMessage(message);
            PluginLog.Verbose($"Sent chat message: '{message}'");
            payload.Completion.SetResult();
        }
    }
}
