using System;
using System.Collections.Generic;
using System.Text;

namespace CashBackMerge.App.CLI;

public record CommandLineOptions
{
    public string Command { get; init; }
    public string ConfigPath { get; init; }
    public int PlayersCount { get; init; }
    public string StrategyName { get; init; }
    public int? Seed { get; init; }
    public CommandLineOptions(
        string command,
        string configPath,
        int playersCount,
        string strategyName,
        int? seed)
    {
        Command = command;
        ConfigPath = configPath;
        PlayersCount = playersCount;
        StrategyName = strategyName;
        Seed = seed;
    }
}