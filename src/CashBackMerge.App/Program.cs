using CashBackMerge.App;
using CashBackMerge.App.core;
using CashBackMerge.App.core.strategies;
using static System.Collections.Specialized.BitVector32;

//var json = """
//    {
//      "boardSize": 4,
//      "movesLimit": 20,
//      "spawnRules": [
//        { "cashback": "1", "probability": "0.85" },
//        { "cashback": "2", "probability": "0.15" }
//      ],
//      "rewardTable": [
//        { "cashback": "2",  "reward": "0" },
//        { "cashback": "4",  "reward": "50" },
//        { "cashback": "8",  "reward": "150" },
//        { "cashback": "16", "reward": "500" },
//        { "cashback": "32", "reward": "2000" }
//      ],
//      "maxRewardCashback": "32",
//      "maxRewardAllowedRate": "0.20"
//    }
//    """;
//var path = Path.GetTempFileName();
//File.WriteAllText(path, json);
//var config = CampaignConfig.LoadConfig(path);
//File.Delete(path);

if (args.Length==0 || args[0] != "simulate")
{
    return;
}
string configPath = null;
int playersCount = 1000;
string strategyName = "RandomStrategy";
int? startSeed = null;
string outputFileName = "report.json";

for (int i = 1; i < args.Length; i++)
{
    switch (args[i].ToLower())
    {
        case "--config":
            if (i + 1 < args.Length) configPath = args[++i];
            break;

        case "--players":
            if (i + 1 < args.Length && int.TryParse(args[++i], out int p))
                playersCount = p;
            break;

        case "--strategy":
            if (i + 1 < args.Length) strategyName = args[++i];
            break;

        case "--seed":
            if (i + 1 < args.Length && int.TryParse(args[++i], out int s))
                startSeed = s;
            break;

        case "--output":
            if (i + 1 < args.Length) outputFileName = args[++i];
            break;

        default:
            Console.WriteLine($"Неизвестный параметр: {args[i]}");
            break;
    }
}
if (string.IsNullOrEmpty(configPath))
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("Ошибка: Параметр --config является обязательным.");
    Console.ResetColor();
    return;
}

if (!File.Exists(configPath))
{
    Console.WriteLine($"[Ошибка] Файл конфигурации не найден по пути: {Path.GetFullPath(configPath)}");
    return;
}

try
{
    CampaignConfig config = CampaignConfig.LoadConfig(configPath);
    string? configDirectory = Path.GetDirectoryName(Path.GetFullPath(configPath));
    if (configDirectory is null)
    {
        throw new InvalidOperationException("Не удалось определить каталог конфигурации.");
    }
    Directory.SetCurrentDirectory(Directory.GetParent(configDirectory)?.FullName
        ?? throw new InvalidOperationException("Не удалось определить каталог приложения."));

    int baseSeed = startSeed ?? new Random().Next(1, 100000);
    IStrategy testStrategy = ResolveStrategies(strategyName, baseSeed);
    SimulationRunner runner = new SimulationRunner(config);
    runner.RunBatchSinulation(testStrategy.Name, playersCount, baseSeed);
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"[Критическая ошибка симуляции]: {ex.Message}");
    Console.ResetColor();
}


IStrategy ResolveStrategies(string name, int seed)
{
    switch (name)
    {
        case "RandomStrategy":
            return new RandomStrategy(new Random(seed));
            break;
        case "CornerStrategy":
            return new CornerStrategy();
            break;
        default:
            throw new ArgumentException($"Неизвестная стратегия: '{name}'. Доступные варианты: random, corner");
            break;
    }
}
//int? seed;
//if (args.Length > 0 && int.TryParse(args[0], out int parsedSeed))
//{
//    seed = parsedSeed;
//}
//else
//{
//    seed = null;
//}

//var session = new GameSession(config, seed);
//ConsoleGame.Run(session);
