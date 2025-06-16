using System.CommandLine.Binding;
using ModSettingCommands.Cmds.Msc.Args.Tmp;
using ModSettingCommands.Cmds.Msc.Options.Tmp;

namespace ModSettingCommands.Cmds.Msc.Binders.Tmp;

public class TmpAssertCmdBinder(TmpAssertCmdOptions options) : AssertCmdBinder<TmpAssertCmdArgs>(options)
{
    protected override TmpAssertCmdArgs GetBoundValue(BindingContext context)
    {
        return base.GetBoundValue(context) with
        {
            Key = context.ParseResult.GetValueForOption(options.Key)
        };
    }
}
