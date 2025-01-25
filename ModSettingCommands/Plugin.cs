using Dalamud.Game;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using ModSettingCommands.Chat;
using ModSettingCommands.Commands;

namespace ModSettingCommands;

public sealed class Plugin : IDalamudPlugin
{
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
    [PluginService] internal static IChatGui ChatGui { get; private set; } = null!;
    [PluginService] internal static IPluginLog PluginLog { get; private set; } = null!;
    [PluginService] internal static ISigScanner SigScanner { get; private set; } = null!;
    [PluginService] internal static IFramework Framework { get; private set; } = null!;

    private ChatSender ChatSender { get; init; }
    private ModSetCommand ModSetCommand { get; init; }
    private IfModSetCommand IfModSetCommand { get; init; }

    public Plugin()
    {
        ChatSender = new(new(SigScanner), Framework, PluginLog);
        ModSetCommand = new(ChatGui, CommandManager, PluginInterface, PluginLog);
        IfModSetCommand = new(ChatGui, ChatSender, CommandManager, PluginInterface, PluginLog);

        PluginInterface.UiBuilder.OpenConfigUi += Noop;
        PluginInterface.UiBuilder.OpenMainUi += Noop;
    }

    public void Dispose()
    {
        IfModSetCommand.Dispose();
        ModSetCommand.Dispose();
        ChatSender.Dispose();
    }

    private static void Noop() { }
}
