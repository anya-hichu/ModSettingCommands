using System.CommandLine.Binding;
using ModSettingCommands.Cmds.Msc.Args;
using ModSettingCommands.Cmds.Msc.Options;

namespace ModSettingCommands.Cmds.Msc.Binders;

public class AddCmdBinder<T> (AddCmdOptions options) : ModCmdBinder<T>(options) where T : AddCmdArgs, new()
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
