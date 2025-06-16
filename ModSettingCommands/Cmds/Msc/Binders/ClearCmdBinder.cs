using ModSettingCommands.Cmds.Msc.Args;
using ModSettingCommands.Cmds.Msc.Options;
using System.CommandLine.Binding;

namespace ModSettingCommands.Cmds.Msc.Binders;

public class ClearCmdBinder<T>(ClearCmdOptions options) : ModCmdBinder<T>(options) where T : ClearCmdArgs, new()
{
    protected override T GetBoundValue(BindingContext context)
    {
        return base.GetBoundValue(context) with
        {
            Group = context.ParseResult.GetValueForOption(options.Group)!
        };
    }
}
