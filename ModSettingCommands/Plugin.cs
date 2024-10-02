using Dalamud.Game;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using ModSettingCommands.Commands;

namespace ModSettingCommands;

public sealed class Plugin : IDalamudPlugin
{
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
    [PluginService] internal static IChatGui ChatGui { get; private set; } = null!;
    [PluginService] internal static IPluginLog PluginLog { get; private set; } = null!;
    [PluginService] internal static ISigScanner SigScanner { get; private set; } = null!;

    private ModSetCommand ModSetCommand { get; init; }
    private IfModSetCommand IfModSetCommand { get; init; }

    public Plugin()
    {
        ModSetCommand = new(ChatGui, CommandManager, PluginInterface, PluginLog);
        IfModSetCommand = new(ChatGui, new(SigScanner), CommandManager, PluginInterface, PluginLog);

        PluginInterface.UiBuilder.OpenConfigUi += Noop;
        PluginInterface.UiBuilder.OpenMainUi += Noop;
    }

    public void Dispose()
    {
        IfModSetCommand.Dispose();
        ModSetCommand.Dispose();
    }

    private static void Noop() { }
}
