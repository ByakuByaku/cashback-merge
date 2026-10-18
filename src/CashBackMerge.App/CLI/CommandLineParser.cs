using CashBackMerge.App.simulation.strategies;
using System;
using System.Collections.Generic;
using System.Text;

namespace CashBackMerge.App.CLI;

public static class CommandLineParser
{
    public static CommandLineOptions Parse(string[] args)
    {
        if (args.Length == 0 || args[0] != "simulate")
        {
            throw new ArgumentException("Укажите команду simulate.");
        }

        string configPath = null;
        int playersCount = 1000;
        string strategyName = "RandomStrategy";
        int? seed = null;

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
                        seed = s;
                    break;
                default:
                    Console.WriteLine($"Неизвестный параметр: {args[i]}");
                    break;
            }
        }
        if (string.IsNullOrEmpty(configPath))
        {
            throw new Exception("Параметр --config обязателен");
        }
        return new CommandLineOptions(
            "simulate", configPath, playersCount, ResolveStrategies(strategyName, seed), seed);
    }
    private static IStrategy ResolveStrategies(string name, int seed)
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
}
