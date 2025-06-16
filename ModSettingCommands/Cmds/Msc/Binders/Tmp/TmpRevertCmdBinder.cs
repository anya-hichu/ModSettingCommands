using System.CommandLine.Binding;
using System.CommandLine;
using ModSettingCommands.Cmds.Msc.Args.Tmp;
using ModSettingCommands.Cmds.Msc.Options.Tmp;

namespace ModSettingCommands.Cmds.Msc.Binders.Tmp;

public class TmpRevertCmdBinder(TmpRevertCmdOptions options) : BinderBase<TmpRevertCmdArgs>
{
    protected override TmpRevertCmdArgs GetBoundValue(BindingContext context)
    {
        return new TmpRevertCmdArgs()
        {
            Collection = context.ParseResult.GetValueForOption(options.Collection)!,
            Key = context.ParseResult.GetValueForOption(options.Key)
        };
    }
}
