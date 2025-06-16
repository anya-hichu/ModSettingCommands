using ModSettingCommands.Cmds.Msc.Args.Tmp;
using ModSettingCommands.Cmds.Msc.Options.Tmp;
using System.CommandLine.Binding;

namespace ModSettingCommands.Cmds.Msc.Binders.Tmp;

public class TmpClearCmdBinder(TmpClearCmdOptions options) : ClearCmdBinder<TmpClearCmdArgs>(options)
{
    protected override TmpClearCmdArgs GetBoundValue(BindingContext context)
    {
        return base.GetBoundValue(context) with
        {
            Key = context.ParseResult.GetValueForOption(options.Key),
            Source = context.ParseResult.GetValueForOption(options.Source)!
        };
    }
}
