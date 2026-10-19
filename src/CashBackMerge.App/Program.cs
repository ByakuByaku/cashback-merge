using CashBackMerge.App.CLI;
using CashBackMerge.App.core;
using CashBackMerge.App.modes;

try
{
    var options = CommandLineParser.Parse(args);
    options = options with { ConfigPath = Path.GetFullPath(options.ConfigPath) };
    var configDirectory = Path.GetDirectoryName(options.ConfigPath)
        ?? throw new InvalidOperationException("Не удалось определить каталог конфигурации.");
    Directory.SetCurrentDirectory(Directory.GetParent(configDirectory)?.FullName
        ?? throw new InvalidOperationException("Не удалось определить каталог приложения."));

    IGameMode mode = options.Command switch
    {
        "play" => CreateConsoleGame(options),
        "simulate" => new SimulationGame(options),
        _ => throw new ArgumentException($"Неизвестная команда: {options.Command}")
    };

    mode.Run();
}
catch (ArgumentException ex)
{
    Console.Error.WriteLine(ex.Message);
}

static IGameMode CreateConsoleGame(CommandLineOptions options)
{
    var config = CampaignConfig.LoadConfig(options.ConfigPath);
    var session = new GameSession(config, seed: options.Seed);
    return new ConsoleGame(session);
}
