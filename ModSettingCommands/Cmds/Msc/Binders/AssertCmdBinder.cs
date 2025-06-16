using System.CommandLine.Binding;
using ModSettingCommands.Cmds.Msc.Args;
using ModSettingCommands.Cmds.Msc.Options;

namespace ModSettingCommands.Cmds.Msc.Binders;

public class AssertCmdBinder<T>(AssertCmdOptions options) : ModCmdBinder<T>(options) where T : AssertCmdArgs, new()
{
    protected override T GetBoundValue(BindingContext context)
    {
        return base.GetBoundValue(context) with
        {
            Group = context.ParseResult.GetValueForOption(options.Group),
            Options = context.ParseResult.GetValueForOption(options.Options),
            Enabled = context.ParseResult.GetValueForOption(options.Enabled),
            Priority = context.ParseResult.GetValueForOption(options.Priority),
            SuccessCmds = context.ParseResult.GetValueForOption(options.SuccessCmds) ?? [],
            FailureCmds = context.ParseResult.GetValueForOption(options.FailureCmds) ?? []
        };
    }
}
