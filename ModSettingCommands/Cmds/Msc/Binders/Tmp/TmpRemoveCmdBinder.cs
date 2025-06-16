using System.CommandLine.Binding;
using ModSettingCommands.Cmds.Msc.Args.Tmp;
using ModSettingCommands.Cmds.Msc.Options.Tmp;

namespace ModSettingCommands.Cmds.Msc.Binders.Tmp;

public class TmpRemoveCmdBinder(TmpRemoveCmdOptions options) : RemoveCmdBinder<TmpRemoveCmdArgs>(options)
{
    protected override TmpRemoveCmdArgs GetBoundValue(BindingContext context)
    {
        return base.GetBoundValue(context) with
        {
            Key = context.ParseResult.GetValueForOption(options.Key),
            Source = context.ParseResult.GetValueForOption(options.Source)!
        };
    }
}
