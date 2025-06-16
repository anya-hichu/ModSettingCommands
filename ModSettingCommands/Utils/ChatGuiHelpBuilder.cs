using System.CommandLine.Help;
using System.CommandLine;
using System.Linq;
using Dalamud.Plugin.Services;

namespace ModSettingCommands.Utils;

public class ChatGuiHelpBuilder(IChatGui chatGui, LocalizationResources localizationResources) : HelpBuilder(localizationResources)
{
    public override void Write(HelpContext context)
    {
        var command = context.Command;

        // TODO: Add colors
        chatGui.Print("Usage:");
        chatGui.Print($"  {command.Name} [options]");

        if (command.Options.Any())
        {
            chatGui.Print($"Options:");
            foreach (var opt in command.Options)
            {
                var aliases = string.Join(", ", opt.Aliases);
                chatGui.Print($"  {aliases,-20} {opt.Description}");
            }
        }

        if (command.Subcommands.Any())
        {
            chatGui.Print("Subcommands:");
            foreach (var sub in command.Subcommands)
            {
                var aliases = string.Join(", ", sub.Aliases);
                chatGui.Print($"  {aliases,-20} {sub.Description}");
            }
        }
    }
}
