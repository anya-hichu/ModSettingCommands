using Dalamud.Game;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using ModSettingCommands.Chat;
using ModSettingCommands.Cmds;

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
    private ModSetCmd ModSetCmd { get; init; }
    private IfModSetCmd IfModSetCmd { get; init; }
    private MscCmd MscCmd { get; init; }

    public Plugin()
    {
        ChatSender = new(new(SigScanner), Framework, PluginLog);
        ModSetCmd = new(ChatGui, CommandManager, PluginInterface, PluginLog);
        IfModSetCmd = new(ChatGui, ChatSender, CommandManager, PluginInterface, PluginLog);
        MscCmd = new(ChatGui, ChatSender, CommandManager, PluginInterface, PluginLog);
    }

    public void Dispose()
    {
        IfModSetCmd.Dispose();
        ModSetCmd.Dispose();
        ChatSender.Dispose();
        MscCmd.Dispose();
    }
}
