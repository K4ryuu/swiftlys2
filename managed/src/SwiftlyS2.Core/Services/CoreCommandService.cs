using System.Runtime;
using System.Reflection;
using System.Runtime.InteropServices;
using Spectre.Console;
using Microsoft.Extensions.Logging;
using SwiftlyS2.Shared;
using SwiftlyS2.Core.Natives;
using SwiftlyS2.Core.Plugins;
using SwiftlyS2.Shared.Commands;

namespace SwiftlyS2.Core.Services;

internal class CoreCommandService
{
    private readonly ILogger<CoreCommandService> logger;
    private readonly ISwiftlyCore core;
    private readonly PluginManager pluginManager;
    private readonly ProfileService profileService;

    public CoreCommandService( ILogger<CoreCommandService> logger, ISwiftlyCore core, PluginManager pluginManager, ProfileService profileService )
    {
        this.logger = logger;
        this.core = core;
        this.pluginManager = pluginManager;
        this.profileService = profileService;
        _ = core.Command.RegisterCommand("sw", OnCommand, true);
    }

    private void OnCommand( ICommandContext context )
    {
        try
        {
            if (context.IsSentByPlayer)
            {
                return;
            }

            var args = context.Args;
            if (args.Length == 0)
            {
                ShowHelp(context);
                return;
            }

            switch (args[0])
            {
                case "help":
                    ShowHelp(context);
                    break;
                case "credits":
                    logger.LogInformation(@"SwiftlyS2 was created and developed by Swiftly Solution SRL and the contributors.
                                            SwiftlyS2 is licensed under the GNU General Public License v3.0 or later.
                                            Website: https://swiftlys2.net/
                                            GitHub: https://github.com/swiftly-solution/swiftlys2");
                    break;
                case "list":
                    var players = core.PlayerManager.GetAllPlayers();
                    var outString = $"Connected players: {core.PlayerManager.PlayerCount}/{core.Engine.GlobalVars.MaxClients}";
                    foreach (var player in players)
                    {
                        outString += $"\n{player.PlayerID}. {player.Controller?.PlayerName}{(player.IsFakeClient ? " (BOT)" : "")} (steamid={player.SteamID})";
                    }
                    logger.LogInformation(outString);
                    break;
                case "status":
                    var uptime = DateTime.Now - System.Diagnostics.Process.GetCurrentProcess().StartTime;
                    var outStrings = $"Uptime: {uptime.Days}d {uptime.Hours}h {uptime.Minutes}m {uptime.Seconds}s";
                    outStrings += $"\nManaged Heap Memory: {GC.GetTotalMemory(false) / 1024.0f / 1024.0f:0.00} MB";
                    outStrings += $"\nLoaded Plugins: {pluginManager.GetPlugins().Count()}";
                    outStrings += $"\nPlayers: {core.PlayerManager.PlayerCount}/{core.Engine.GlobalVars.MaxClients}";
                    outStrings += $"\nMap: {core.Engine.GlobalVars.MapName.Value}";
                    logger.LogInformation(outStrings);
                    break;
                case "version":
                    var outVersion = $"SwiftlyS2 Version: {NativeEngineHelpers.GetNativeVersion()}";
                    outVersion += $"\nSwiftlyS2 Managed Version: {Assembly.GetExecutingAssembly().GetName().Version}";
                    outVersion += $"\nSwiftlyS2 Runtime Version: {Environment.Version}";
                    outVersion += $"\nSwiftlyS2 C++ Version: C++23";
                    outVersion += $"\nSwiftlyS2 .NET Version: {RuntimeInformation.FrameworkDescription}";
                    outVersion += $"\nGitHub URL: https://github.com/swiftly-solution/swiftlys2";
                    logger.LogInformation(outVersion);
                    break;
                case "gc":
                    if (context.IsSentByPlayer)
                    {
                        context.Reply("This command can only be executed from the server console.");
                        return;
                    }
                    var outGc = "Garbage Collection Information:";
                    outGc += $"\n  - Total Memory: {GC.GetTotalMemory(false) / 1024.0f / 1024.0f:0.00} MB";
                    outGc += $"\n  - Is Server GC: {GCSettings.IsServerGC}";
                    outGc += $"\n  - Max Generation: {GC.MaxGeneration}";
                    for (var i = 0; i <= GC.MaxGeneration; i++)
                    {
                        outGc += $"\n    - Generation {i} Collection Count: {GC.CollectionCount(i)}";
                    }
                    outGc += $"\n  - Latency Mode: {GCSettings.LatencyMode}";
                    logger.LogInformation(outGc);
                    break;
                case "plugins":
                    if (context.IsSentByPlayer)
                    {
                        context.Reply("This command can only be executed from the server console.");
                        return;
                    }
                    PluginCommand(context);
                    break;
                case "profiler":
                    if (context.IsSentByPlayer)
                    {
                        context.Reply("This command can only be executed from the server console.");
                        return;
                    }
                    ProfilerCommand(context);
                    break;
                case "confilter":
                    if (context.IsSentByPlayer)
                    {
                        context.Reply("This command can only be executed from the server console.");
                        return;
                    }
                    ConfilterCommand(context);
                    break;
                default:
                    ShowHelp(context);
                    break;
            }
        }
        catch (Exception e)
        {
            if (!GlobalExceptionHandler.Handle(e))
            {
                return;
            }
            logger.LogError(e, "Error executing command");
        }
    }

    private static void ShowHelp( ICommandContext context )
    {
        var table = new Table().AddColumn("Command").AddColumn("Description");
        table = table.AddRow("credits", "List Swiftly credits");
        table = table.AddRow("help", "Show the help for Swiftly Commands");
        table = table.AddRow("list", "Show the list of online players");
        table = table.AddRow("status", "Show the status of the server");
        if (!context.IsSentByPlayer)
        {
            table = table.AddRow("confilter", "Console Filter Menu");
            table = table.AddRow("plugins", "Plugin Management Menu");
            table = table.AddRow("gc", "Show garbage collection information on managed");
            table = table.AddRow("profiler", "Profiler Menu");
        }
        table = table.AddRow("version", "Display Swiftly version");
        AnsiConsole.Write(table);
    }

    private void ConfilterCommand( ICommandContext context )
    {
        var args = context.Args;
        if (args.Length == 1)
        {
            var table = new Table().AddColumn("Command").AddColumn("Description");
            table = table.AddRow("enable", "Enable console filtering");
            table = table.AddRow("disable", "Disable console filtering");
            table = table.AddRow("status", "Show the status of the console filter");
            table = table.AddRow("reload", "Reload console filter configuration");
            AnsiConsole.Write(table);
            return;
        }

        switch (args[1])
        {
            case "enable":
                if (!core.ConsoleOutput.IsFilterEnabled()) core.ConsoleOutput.ToggleFilter();
                logger.LogInformation("Console filtering has been enabled.");
                break;
            case "disable":
                if (core.ConsoleOutput.IsFilterEnabled()) core.ConsoleOutput.ToggleFilter();
                logger.LogInformation("Console filtering has been disabled.");
                break;
            case "status":
                logger.LogInformation($"Console filtering is currently {(core.ConsoleOutput.IsFilterEnabled() ? "enabled" : "disabled")}.\nBelow are some statistics for the filtering process:\n{core.ConsoleOutput.GetCounterText()}");
                break;
            case "reload":
                core.ConsoleOutput.ReloadFilterConfiguration();
                logger.LogInformation("Console filter configuration reloaded.");
                break;
            default:
                logger.LogWarning("Unknown command");
                break;
        }
    }

    private void ProfilerCommand( ICommandContext context )
    {
        var args = context.Args;
        if (args.Length == 1)
        {
            var table = new Table().AddColumn("Command").AddColumn("Description");
            table = table.AddRow("enable", "Enable the profiler");
            table = table.AddRow("disable", "Disable the profiler");
            table = table.AddRow("status", "Show the status of the profiler");
            table = table.AddRow("save", "Save the profiler data to a file");
            AnsiConsole.Write(table);
            return;
        }

        switch (args[1])
        {
            case "enable":
                profileService.Enable();
                logger.LogInformation("The profiler has been enabled.");
                break;
            case "disable":
                profileService.Disable();
                logger.LogInformation("The profiler has been disabled.");
                break;
            case "status":
                logger.LogInformation($"Profiler is currently {(profileService.IsEnabled() ? "enabled" : "disabled")}.");
                break;
            case "save":
                var pluginId = args.Length >= 3 ? args[2] : "";
                var basePath = Environment.GetEnvironmentVariable("SWIFTLY_MANAGED_ROOT")!;
                if (!File.Exists(Path.Combine(basePath, "profilers")))
                {
                    _ = Directory.CreateDirectory(Path.Combine(basePath, "profilers"));
                }

                var guid = Guid.NewGuid();
                File.WriteAllText(Path.Combine(basePath, "profilers", $"profiler.{guid}.{(pluginId == "" ? "core" : pluginId)}.json"), profileService.GenerateJSONPerformance(pluginId));
                logger.LogInformation($"Profile saved to {Path.Combine(basePath, "profilers", $"profiler.{guid}.{(pluginId == "" ? "core" : pluginId)}.json")}");
                break;
            default:
                logger.LogWarning("Unknown command");
                break;
        }
    }

    private void PluginCommand( ICommandContext context )
    {
        var args = context.Args;
        if (args.Length == 1)
        {
            var table = new Table().AddColumn("Command").AddColumn("Description");
            table = table.AddRow("list", "List all plugins");
            table = table.AddRow("load", "Load a plugin");
            table = table.AddRow("unload", "Unload a plugin");
            table = table.AddRow("reload", "Reload a plugin");
            AnsiConsole.Write(table);
            return;
        }

        switch (args[1])
        {
            case "list":
                var table = new Table().AddColumn("Name").AddColumn("Status").AddColumn("Version").AddColumn("Author").AddColumn("Website");
                foreach (var plugin in pluginManager.GetPlugins())
                {
                    table = table.AddRow(plugin.Metadata?.Id ?? "<UNKNOWN>", plugin.Status?.ToString() ?? "Unknown", plugin.Metadata?.Version ?? "<UNKNOWN>", plugin.Metadata?.Author ?? "<UNKNOWN>", plugin.Metadata?.Website ?? "<UNKNOWN>");
                }
                AnsiConsole.Write(table);
                break;
            case "load":
                if (args.Length < 3)
                {
                    logger.LogWarning("Usage: sw plugins load <pluginId>");
                    return;
                }
                _ = pluginManager.LoadPluginById(args[2]);
                break;
            case "unload":
                if (args.Length < 3)
                {
                    logger.LogWarning("Usage: sw plugins unload <pluginId>");
                    return;
                }
                _ = pluginManager.UnloadPluginById(args[2]);
                break;
            case "reload":
                if (args.Length < 3)
                {
                    logger.LogWarning("Usage: sw plugins reload <pluginId>");
                    return;
                }
                pluginManager.ReloadPluginById(args[2], true);
                break;
            default:
                logger.LogWarning("Unknown command");
                break;
        }
    }
}