using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using ModSettingCommands.Chat;
using ModSettingCommands.Cmds.Msc;
using ModSettingCommands.Cmds.Msc.Args;
using ModSettingCommands.Cmds.Msc.Args.Tmp;
using ModSettingCommands.Cmds.Msc.Binders;
using ModSettingCommands.Cmds.Msc.Binders.Tmp;
using ModSettingCommands.Cmds.Msc.Options;
using ModSettingCommands.Cmds.Msc.Options.Tmp;
using ModSettingCommands.Cmds.Msc.Tmp;
using ModSettingCommands.Utils;
using Penumbra.Api.Enums;
using Penumbra.Api.IpcSubscribers;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ModSettingCommands.Cmds;

public partial class MscCmd : BaseModSetCmd
{
    private static readonly string COMMAND = "/msc";
    private static readonly string COMMAND_HELP_MESSAGE = "Check help message for syntax";

    private static readonly int DEFAULT_MESSAGE_INTERVAL_MS = 60;

    private static readonly string OPEN_TAG = "<";
    private static readonly string CLOSE_TAG = ">";

    [GeneratedRegexAttribute(@"(?<=^|[^[])\[(?=[^[])")]
    private static partial Regex OpenEscapedTagGeneratedRegex();

    [GeneratedRegexAttribute(@"(?<=[^]])\](?=[^]]|$)")]
    private static partial Regex CloseEscapedTagGeneratedRegex();

    [GeneratedRegexAttribute(@"<wait\.(\d+)>")]
    private static partial Regex WaitTimeGeneratedRegex();

    private ChatSender ChatSender { get; init; }

    private ChatGuiConsole ChatGuiConsole { get; init; }

    private IPluginLog PluginLog { get; init; }

    private Parser Parser { get; init; }

    private GetCurrentModSettingsWithTemp GetCurrentModSettingsWithTemp  { get; init; }


    private TrySetMod TrySetMod { get; init; }
    private TrySetModPriority TrySetModPriority { get; init; }
    private TrySetModSettings TrySetModSettings { get; init; }
    


    private SetTemporaryModSettings SetTemporaryModSettings { get; init; }
    private RemoveAllTemporaryModSettings RemoveAllTemporaryModSettings { get; init; }

    private CancellationTokenSource CancellationTokenSource { get; set; } = new();

    public MscCmd(IChatGui chatGui, ChatSender chatSender, ICommandManager commandManager, IDalamudPluginInterface pluginInterface, IPluginLog pluginLog) : base(COMMAND, COMMAND_HELP_MESSAGE, chatGui, commandManager, pluginInterface)
    {
        ChatGui = chatGui;
        ChatGuiConsole = new(chatGui);
        ChatSender = chatSender;
        CommandManager = commandManager;
        PluginLog = pluginLog;

        GetCurrentModSettingsWithTemp = new(pluginInterface);

        TrySetMod = new(pluginInterface);
        TrySetModPriority = new(pluginInterface);
        TrySetModSettings = new(pluginInterface);
        

        SetTemporaryModSettings = new(pluginInterface);
        RemoveAllTemporaryModSettings = new(pluginInterface);

        var rootCommand = new Command(COMMAND, "Utility to work with mod settings");

        var collectionOption = new Option<string>(["--collection", "-c"], "Collection") { IsRequired = true };
        var modDirOption = new Option<string>(["--mod-dir", "-m"], "Mod directory") { IsRequired = true };
        var modNameOption = new Option<string?>(["--mod-name", "-n"], "Mod name (optional)");

        var groupOption = new Option<string>(["--group", "-g"], "Group") { IsRequired = true };
        var maybeGroupOption = new Option<string?>(["--group", "-g"], "Group (optional)");
        var optionsOption = new Option<List<string>>(["--option", "-o"], "Options") { IsRequired = true, AllowMultipleArgumentsPerToken = true, Arity = ArgumentArity.OneOrMore };
        var maybeOptionsOption = new Option<List<string>?>(["--option", "-o"], "Options (optional)") { AllowMultipleArgumentsPerToken = true, Arity = ArgumentArity.OneOrMore };

        var inheritOption = new Option<bool?>(["--inherit", "-i"], "Inherit flag (optional)");
        var priorityOption = new Option<int?>(["--priority", "-p"], "Priority (optional)");
        var enabledOption = new Option<bool?>(["--enabled", "-e"], "Enabled flag (optional)");

        var successCmdsOption = new Option<List<string>>(["--success-cmd", "-sc"], "Success commands (optional)") { AllowMultipleArgumentsPerToken = true };
        var failureCmdsOption = new Option<List<string>>(["--failure-cmd", "-fc"], "Failure commands (optional)") { AllowMultipleArgumentsPerToken = true };

        var keyOption = new Option<int?>(["--key", "-k"], "Key (optional, defaults: 0)");
        var sourceOption = new Option<string>(["--source", "-s"], "Source") { IsRequired = true };

        var modCmdOptions = new ModCmdOptions(collectionOption, modDirOption, modNameOption);
        var setCmdOptions = new SetCmdOptions(modCmdOptions, maybeGroupOption, maybeOptionsOption, inheritOption, priorityOption, enabledOption);
        rootCommand.AddCommand(new SetCmd(setCmdOptions).Tap(cmd => cmd.SetHandler(
            HandleSet,
            new SetCmdBinder<SetCmdArgs>(setCmdOptions)
        )));
        var clearCmdOptions = new ClearCmdOptions(modCmdOptions, groupOption);
        rootCommand.AddCommand(new ClearCmd(clearCmdOptions).Tap(cmd => cmd.SetHandler(
            HandleClear,
            new ClearCmdBinder<ClearCmdArgs>(clearCmdOptions)
        )));
        var addCmdOptions = new AddCmdOptions(modCmdOptions, groupOption, optionsOption);
        rootCommand.AddCommand(new AddCmd(addCmdOptions).Tap(cmd => cmd.SetHandler(
            HandleAdd,
            new AddCmdBinder<AddCmdArgs>(addCmdOptions)            
        )));
        var removeCmdOptions = new RemoveCmdOptions(modCmdOptions, groupOption, optionsOption);
        rootCommand.AddCommand(new RemoveCmd(removeCmdOptions).Tap(cmd => cmd.SetHandler(
            HandleRemove,
            new RemoveCmdBinder<RemoveCmdArgs>(removeCmdOptions)
        )));
        var assertCmdOptions = new AssertCmdOptions(modCmdOptions, maybeGroupOption, maybeOptionsOption, enabledOption, priorityOption, successCmdsOption, failureCmdsOption);
        rootCommand.AddCommand(new AssertCmd(assertCmdOptions).Tap(cmd => cmd.SetHandler(
            HandleAssert,
            new AssertCmdBinder<AssertCmdArgs>(assertCmdOptions)
        )));

        var tmpCommand = new Command("tmp", "Temporary operations");
        var tmpSetCmdOptions = new TmpSetCmdOptions(setCmdOptions, keyOption, sourceOption);
        tmpCommand.AddCommand(new TmpSetCmd(tmpSetCmdOptions).Tap(cmd => cmd.SetHandler(
            HandleTmpSet,
            new TmpSetCmdBinder(tmpSetCmdOptions)
        )));
        var tmpClearOptions = new TmpClearCmdOptions(clearCmdOptions, keyOption, sourceOption);
        tmpCommand.AddCommand(new TmpClearCmd(tmpClearOptions).Tap(cmd => cmd.SetHandler(
            HandleTmpClear,
            new TmpClearCmdBinder(tmpClearOptions)
        )));
        var tmpAddOptions = new TmpAddCmdOptions(addCmdOptions, keyOption, sourceOption);
        tmpCommand.AddCommand(new TmpAddCmd(tmpAddOptions).Tap(cmd => cmd.SetHandler(
            HandleTmpAdd,
            new TmpAddCmdBinder(tmpAddOptions)
        )));
        var tmpRemoveOptions = new TmpRemoveCmdOptions(removeCmdOptions, keyOption, sourceOption);
        tmpCommand.AddCommand(new TmpRemoveCmd(tmpRemoveOptions).Tap(cmd => cmd.SetHandler(
            HandleTmpRemove,
            new TmpRemoveCmdBinder(tmpRemoveOptions)
        )));
        var tmpAssertOptions = new TmpAssertCmdOptions(assertCmdOptions, keyOption, sourceOption);
        tmpCommand.AddCommand(new TmpAssertCmd(tmpAssertOptions).Tap(cmd => cmd.SetHandler(
            HandleTmpAssert,
            new TmpAssertCmdBinder(tmpAssertOptions)
        )));
        var tmpRevertOptions = new TmpRevertCmdOptions(collectionOption, keyOption);
        tmpCommand.AddCommand(new TmpRevertCmd(tmpRevertOptions).Tap(cmd => cmd.SetHandler(
            HandleTmpRevert,
            new TmpRevertCmdBinder(tmpRevertOptions)
        )));

        rootCommand.AddCommand(tmpCommand);

        var builder = new CommandLineBuilder(rootCommand)
            .UseHelpBuilder(_ => new ChatGuiHelpBuilder(ChatGui, LocalizationResources.Instance))
            .UseHelp()
            .UseParseErrorReporting()
            .UseExceptionHandler()
            .CancelOnProcessTermination();

        Parser = builder.Build();
    }

    public override void Dispose()
    {
        base.Dispose();
        CancellationTokenSource.Cancel();
    }

    protected override void Handle(string command, string args)
    {
        PluginLog.Debug($"{nameof(Handle)} called with {command} and {args}");
        Parser.Invoke(Arguments.SplitCommandLine(args), ChatGuiConsole);
    }

    private void HandleSet(SetCmdArgs args)
    {
        PluginLog.Debug($"{nameof(HandleSet)} called with {args}");
        if (!TryUpdateSetting(args.Collection, args.ModDir, args.ModName, args.Enabled, args.Priority, args.Group, options => args.Options ?? options))
        {
            PluginLog.Error($"Failed to update settings in {nameof(HandleSet)} using {args}");
        }
    }

    private void HandleClear(ClearCmdArgs args)
    {
        PluginLog.Debug($"{nameof(HandleClear)} called with {args}");
        if (!TryUpdateSetting(args.Collection, args.ModDir, args.ModName, null, null, args.Group, _ => []))
        {
            PluginLog.Error($"Failed to update settings for {nameof(HandleClear)} using {args}");
        }
    }

    private void HandleAdd(AddCmdArgs args)
    {
        PluginLog.Debug($"{nameof(HandleAdd)} called with {args}");
        if (!TryUpdateSetting(args.Collection, args.ModDir, args.ModName, null, null, args.Group, options => [.. options.Union(args.Options)]))
        {
            PluginLog.Error($"Failed to update settings in {nameof(HandleAdd)} using {args}");
        }
    }

    private void HandleRemove(RemoveCmdArgs args)
    {
        PluginLog.Debug($"{nameof(HandleRemove)} called with {args}");
        if (!TryUpdateSetting(args.Collection, args.ModDir, args.ModName, null, null, args.Group, options => [.. options.Except(args.Options)]))
        {
            PluginLog.Error($"Failed to update settings in {nameof(HandleRemove)} using {args}");
        }
    }

    private bool TryUpdateSetting(string collection, string modDir, string? modName, bool? enabled, int? priority, string? group, Func<List<string>, List<string>>? transform)
    {
        if (!TryGetCollectionId(collection, out var collectionId))
        {
            return false;
        }

        var (ec1, data) = GetCurrentModSettingsWithTemp.Invoke(collectionId, modDir, modName ?? string.Empty, true, true);
        if (CheckError(ec1))
        {
            return false;
        }
        if (!data.HasValue)
        {
            PluginLog.Warning($"Failed to retrieve mod settings for {modDir} in collection {collection} using defaults instead");
            data = (false, 0, group != null ? new() { { group, [] } } : [], false, false);
        }
        var (_, _, settings, _, _) = data.Value;


        if (enabled.HasValue)
        {
            var ec2 = TrySetMod.Invoke(collectionId, modDir, enabled.Value, modName ?? string.Empty);
            if (CheckError(ec2))
            {
                return false;
            }
        }


        if (priority.HasValue)
        {
            var ec3 = TrySetModPriority.Invoke(collectionId, modDir, priority.Value, modName ?? string.Empty);
            if (CheckError(ec3))
            {
                return false;
            }
        }

        if (group != null)
        {
            if (!settings.TryGetValue(group, out var options))
            {
                ChatGui.PrintError($"Failed to retrieve options on group {group} for {modDir} in collection {collection}");
                return false;
            }

            var ec4 = TrySetModSettings.Invoke(collectionId, modDir, group, transform != null ? transform(options!) : options!, modName ?? string.Empty);
            if (CheckError(ec4))
            {
                return false;
            }
        }

        return true;
    }

    private void HandleAssert(AssertCmdArgs args)
    {
        PluginLog.Debug($"{nameof(HandleAssert)} called with {args}");
        if (!TryGetCollectionId(args.Collection, out var collectionId))
        {
            return;
        }
        var (ec1, data) = GetCurrentModSettingsWithTemp.Invoke(collectionId, args.ModDir, args.ModName ?? string.Empty, true, true);
        if (CheckError(ec1))
        {
            return;
        }

        if (!data.HasValue)
        {
            PluginLog.Warning($"Failed to retrieve mod settings for {args.ModDir} in collection {args.Collection} using defaults instead");
            data = (false, 0, args.Group != null ? new() { { args.Group, [] } } : [], false, false);
        }

        var (enabled, priority, settings, _, _) = data.Value;

        var matchEnabled = !args.Enabled.HasValue || args.Enabled.Value == enabled;
        var matchPriority = !args.Priority.HasValue || args.Priority.Value == priority;

        var matchOptions = true;
        if (args.Group != null)
        {
            if (!settings.TryGetValue(args.Group, out var currentOptions))
            {
                ChatGui.PrintError($"Failed to retrieve options for group {args.Group} on {args.ModDir} in collection {args.Collection}");
                return;
            }
            matchOptions = currentOptions.ToHashSet().SetEquals(args.Options ?? []);
        }
        ExecuteCmdsInTask(matchEnabled && matchPriority && matchOptions ? args.SuccessCmds : args.FailureCmds);
    }

    private void HandleTmpSet(TmpSetCmdArgs args)
    {
        PluginLog.Debug($"{nameof(HandleTmpSet)} called with {args}");
        if (!TryUpdateTmpSetting(args.Collection, args.ModDir, args.ModName, args.Enabled, args.Priority, args.Group, args.Key, args.Source, options => args.Options ?? options))
        {
            PluginLog.Error($"Failed to update settings in {nameof(HandleTmpSet)} using {args}");
        }
    }

    private void HandleTmpClear(TmpClearCmdArgs args)
    {
        PluginLog.Debug($"{nameof(HandleTmpClear)} called with {args}");
        if (!TryUpdateTmpSetting(args.Collection, args.ModDir, args.ModName, null, null, args.Group, args.Key, args.Source, _ => []))
        {
            PluginLog.Error($"Failed to update settings in {nameof(HandleTmpClear)} using {args}");
        }
    }

    private void HandleTmpAdd(TmpAddCmdArgs args)
    {
        PluginLog.Debug($"{nameof(HandleTmpAdd)} called with {args}");
        if (!TryUpdateTmpSetting(args.Collection, args.ModDir, args.ModName, null, null, args.Group, args.Key, args.Source, options => [.. options.Union(args.Options)]))
        {
            PluginLog.Error($"Failed to update settings in {nameof(HandleTmpAdd)} using {args}");
        }
    }

    private void HandleTmpRemove(TmpRemoveCmdArgs args)
    {
        PluginLog.Debug($"{nameof(HandleTmpRemove)} called with {args}");
        if (!TryUpdateTmpSetting(args.Collection, args.ModDir, args.ModName, null, null, args.Group, args.Key, args.Source, options => [.. options.Except(args.Options)]))
        {
            PluginLog.Error($"Failed to update settings in {nameof(HandleTmpRemove)} using {args}");
        }
    }

    private bool TryUpdateTmpSetting(string collection, string modDir, string? modName, bool? enabled, int? priority, string? group, int? key, string source, Func<List<string>, List<string>>? transform)
    {
        if (!TryGetCollectionId(collection, out var collectionId))
        {
            return false;
        }

        var (ec1, data) = GetCurrentModSettingsWithTemp.Invoke(collectionId, modDir, modName ?? string.Empty, false, false, key ?? 0);
        if (CheckError(ec1))
        {
            return false;
        }
        if (!data.HasValue)
        {
            PluginLog.Warning($"Failed to retrieve mod settings for {modDir} in collection {collection} using defaults instead");
            data = (false, 0, group != null ? new() { { group, [] } } : [], false, false);
        }
        var (currentEnabled, currentPriority, currentSettings, _, _) = data.Value;
        var settings = currentSettings.ToDictionary(setting => setting.Key, setting => (IReadOnlyList<string>)(transform != null && setting.Key == group ? transform(setting.Value) : setting.Value)).AsReadOnly();
        var ec2 = SetTemporaryModSettings.Invoke(collectionId, modDir, false, enabled.HasValue ? enabled.Value : currentEnabled, priority.HasValue ? priority.Value : currentPriority, settings, source, key ?? 0, modName ?? string.Empty);
        if (CheckError(ec2))
        {
            return false;
        }

        return true;
    }

    private void HandleTmpAssert(TmpAssertCmdArgs args)
    {
        PluginLog.Debug($"{nameof(HandleTmpAssert)} called with {args}");
        if (!TryGetCollectionId(args.Collection, out var collectionId))
        {
            return;
        }
        var (ec1, data) = GetCurrentModSettingsWithTemp.Invoke(collectionId, args.ModDir, args.ModName ?? string.Empty, false, false, args.Key ?? 0);

        if (CheckError(ec1))
        {
            return;
        }

        if (!data.HasValue)
        {
            PluginLog.Warning($"Failed to retrieve mod settings for {args.ModDir} in collection {args.Collection} using defaults instead");
            data = (false, 0, args.Group != null ? new() { { args.Group, [] } } : [], false, false);
        }

        var (enabled, priority, settings, _, _) = data.Value;
        var matchEnabled = !args.Enabled.HasValue || args.Enabled.Value == enabled;
        var matchPriority = !args.Priority.HasValue || args.Priority.Value == priority;

        var matchOptions = true;
        if (args.Group != null)
        {
            if (!settings.TryGetValue(args.Group, out var currentOptions))
            {
                ChatGui.PrintError($"Failed to retrieve options for group {args.Group} on {args.ModDir} in collection {args.Collection}");
                return;
            }

            matchOptions = currentOptions.ToHashSet().SetEquals(args.Options ?? []);
        }

        ExecuteCmdsInTask(matchEnabled && matchPriority && matchOptions ? args.SuccessCmds : args.FailureCmds);
    }

    private void HandleTmpRevert(TmpRevertCmdArgs args)
    {
        PluginLog.Debug($"{nameof(HandleTmpRevert)} called with {args}");
        if (!TryGetCollectionId(args.Collection, out var collectionId))
        {
            return;
        }
        var ec = RemoveAllTemporaryModSettings.Invoke(collectionId, args.Key ?? 0);
        if (CheckError(ec))
        {
            return;
        }
    }

    private bool CheckError(PenumbraApiEc exitCode)
    {
        if (exitCode == PenumbraApiEc.Success || exitCode == PenumbraApiEc.NothingChanged)
        {
            return false;
        }
        ChatGui.PrintError($"Failed with error code '{exitCode}'");
        return true;
    }

    private static string UnescapePlaceholders(string message)
    {
        return CloseEscapedTagGeneratedRegex()
            .Replace(OpenEscapedTagGeneratedRegex().Replace(message, OPEN_TAG), CLOSE_TAG)
            .Replace("[[", "[").Replace("]]", "]");
    }

    private void ExecuteCmdsInTask(IEnumerable<string> cmds)
    {
        var token = CancellationTokenSource.Token;
        Task.Run(() => {
            foreach (var cmd in cmds)
            {
                token.ThrowIfCancellationRequested();

                var unescapedCommand = UnescapePlaceholders(cmd);
                var waitTimeMatch = WaitTimeGeneratedRegex().Match(unescapedCommand);
                var cmdWithoutWait = waitTimeMatch.Success ? WaitTimeGeneratedRegex().Replace(unescapedCommand, string.Empty) : unescapedCommand;

                Task.WaitAny([ChatSender.SendOnFrameworkThread(cmdWithoutWait)], token);

                if (waitTimeMatch.Success)
                {
                    var waitTimeValue = waitTimeMatch.Groups[1].Value;
                    PluginLog.Verbose($"Pausing execution #{Task.CurrentId} after '{cmdWithoutWait}' for {waitTimeValue} sec(s)");
                    Thread.Sleep(int.Parse(waitTimeValue) * 1000);
                }
                else
                {
                    Thread.Sleep(DEFAULT_MESSAGE_INTERVAL_MS);
                }
            }
        }, token);
    }
}
