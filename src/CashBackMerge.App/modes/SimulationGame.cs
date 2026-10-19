using CashBackMerge.App.CLI;
using CashBackMerge.App.simulation;
using CashBackMerge.App.core;
using System;
using System.Collections.Generic;
using System.Text;

namespace CashBackMerge.App.modes;

internal class SimulationGame : IGameMode
{
    private CommandLineOptions _options;
    public SimulationGame(CommandLineOptions options)
    {
        _options = options;
    }
    public void Run()
    {
        try
        {
            var config = CampaignConfig.LoadConfig(_options.ConfigPath);

            var runner = new SimulationRunner(config);
            runner.RunBatchSinulation(_options.StrategyName, _options.PlayersCount, _options.Seed);
        }
        catch (ArgumentException ex)
        {
            Console.Error.WriteLine(ex.Message);
        }
    }
}
