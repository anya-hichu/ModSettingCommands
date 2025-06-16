using System.Collections.Generic;
using System.CommandLine.Binding;
using ModSettingCommands.Cmds.Msc.Args;
using ModSettingCommands.Cmds.Msc.Options;

namespace ModSettingCommands.Cmds.Msc.Binders;

public class RemoveCmdBinder<T>(RemoveCmdOptions options) : ModCmdBinder<T>(options) where T : RemoveCmdArgs, new()
{
    protected override T GetBoundValue(BindingContext context)
    {
        return base.GetBoundValue(context) with
        {
            Group = context.ParseResult.GetValueForOption(options.Group)!,
            Options = context.ParseResult.GetValueForOption(options.Options) ?? []
        };
    }
}
