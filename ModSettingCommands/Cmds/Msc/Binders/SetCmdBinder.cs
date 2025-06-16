using System.Collections.Generic;
using System.CommandLine.Binding;
using ModSettingCommands.Cmds.Msc.Args;
using ModSettingCommands.Cmds.Msc.Options;

namespace ModSettingCommands.Cmds.Msc.Binders;

public class SetCmdBinder<T>(SetCmdOptions options) : ModCmdBinder<T>(options) where T : SetCmdArgs, new()
{
    protected override T GetBoundValue(BindingContext context)
    {
        return base.GetBoundValue(context) with
        {
            Group = context.ParseResult.GetValueForOption(options.Group),
            Options = context.ParseResult.GetValueForOption(options.Options),
            Inherit = context.ParseResult.GetValueForOption(options.Inherit),
            Enabled = context.ParseResult.GetValueForOption(options.Enabled),
            Priority = context.ParseResult.GetValueForOption(options.Priority)
        };
    }
}
