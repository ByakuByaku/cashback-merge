using System;
using System.Collections.Generic;
using System.Text;

namespace CashBackMerge.App.CLI;

public record CommandLineOptions
{
    public string Command { get; init; }
    public required string ConfigPath { get; init; }
    public int PlayersCount { get; init; } = 1000;
    public string StrategyName { get; init; } = "RandomStrategy";
    public int? Seed { get; init; }
    public CommandLineOptions(string command)
    {
        Command = command;
    }
}
