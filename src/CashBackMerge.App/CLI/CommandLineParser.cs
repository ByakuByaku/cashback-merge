namespace CashBackMerge.App.CLI;

public static class CommandLineParser
{
    public static CommandLineOptions Parse(string[] args)
    {
        if (args.Length < 1 || (args[0] != "play" && args[0] != "simulate"))
        {
            throw new ArgumentException("Укажите команду play или simulate.");
        }

        var command = args[0].ToLower();
        string? configPath = null;
        int? seed = null;
        var playersCount = 1000;
        var strategyName = "RandomStrategy";

        for (var i = 1; i < args.Length; i++)
        {
            var option = args[i].ToLower();
            if (i + 1 >= args.Length)
            {
                throw new ArgumentException($"После {option} нужно указать значение.");
            }
            var value = args[++i];
            if (value.StartsWith("--"))
            {
                throw new ArgumentException($"После {option} нужно указать значение.");
            }
            switch (option)
            {
                case "--config":
                    configPath = value;
                    break;
                case "--seed":
                    if (!int.TryParse(value, out var parsedSeed))
                        throw new ArgumentException("--seed должен быть целым числом.");
                    seed = parsedSeed;
                    break;
                case "--players" when command == "simulate":
                    if (!int.TryParse(value, out playersCount) || playersCount <= 0)
                        throw new ArgumentException("--players должен быть положительным целым числом.");
                    break;
                case "--strategy" when command == "simulate":
                    strategyName = value;
                    break;
                default:
                    throw new ArgumentException($"Неизвестный параметр для {command}: {option}");
            }
        }

        if (string.IsNullOrWhiteSpace(configPath))
        {
            throw new ArgumentException($"Для команды {command} требуется параметр --config.");
        }

        return new CommandLineOptions(command)
        {
            ConfigPath = configPath,
            Seed = seed,
            PlayersCount = playersCount,
            StrategyName = strategyName
        };
    }
}
