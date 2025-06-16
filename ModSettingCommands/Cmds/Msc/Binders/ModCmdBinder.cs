using ModSettingCommands.Cmds.Msc.Args;
using ModSettingCommands.Cmds.Msc.Options;
using System.CommandLine.Binding;

namespace ModSettingCommands.Cmds.Msc.Binders;

public abstract class ModCmdBinder<T>(ModCmdOptions options) : BinderBase<T> where T : ModCmdArgs, new()
{
    protected override T GetBoundValue(BindingContext context)
    {
        return new T
        {
            Collection = context.ParseResult.GetValueForOption(options.Collection)!,
            ModDir = context.ParseResult.GetValueForOption(options.ModDir)!,
            ModName = context.ParseResult.GetValueForOption(options.ModName)
        };
    }
}
