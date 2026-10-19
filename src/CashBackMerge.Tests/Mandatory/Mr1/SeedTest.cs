using CashBackMerge.App.CLI;
using CashBackMerge.App.core;
using CashBackMerge.App.simulation;
using System.Text.Json.Nodes;
namespace CashBackMerge.Tests.Mandatory.Mr1;

/// <summary>Обязательные тесты: воспроизводимость по seed.</summary>
public class SeedTest
{
    [Fact(DisplayName = "Одинаковый конфиг и seed дают одинаковое начальное поле")]
    public void SameConfigAndSeedProduceSameInitialBoard()
    {
        var path = Path.GetTempFileName();
        File.WriteAllText(path, """
            {
              "boardSize": 4,
              "movesLimit": 20,
              "spawnRules": [
                { "cashback": "1", "probability": "0.85" },
                { "cashback": "2", "probability": "0.15" }
              ]
            }
            """);
        var config = CampaignConfig.LoadConfig(path);
        var session1 = new GameSession(config, seed: 42);
        var session2 = new GameSession(config, seed: 42);
        Assert.Equal(session1.GameField.ToString(), session2.GameField.ToString());
        File.Delete(path);
    }

    [Fact(DisplayName = "Одинаковый конфиг, seed и стратегия дают одинаковый отчёт симуляции")]
    public void SameInputsProduceSameSimulationReport()
    {
        CommandLineOptions CommandLineOptions = new CommandLineOptions($"simulate --config {Path.Combine(
            AppContext.BaseDirectory,
            "configs",
            "CampaignConfig.json")} --players 1 --strategy CornerStrategy --seed 5555")
        {
            ConfigPath = Path.Combine(AppContext.BaseDirectory, "configs", "CampaignConfig.json")
        };
        var config = CampaignConfig.LoadConfig(CommandLineOptions.ConfigPath);
        var simulationRunner = new SimulationRunner(config);
        var simulationRunner2 = new SimulationRunner(config);
        simulationRunner.RunBatchSinulation(CommandLineOptions.StrategyName, CommandLineOptions.PlayersCount, CommandLineOptions.Seed);
        simulationRunner2.RunBatchSinulation(CommandLineOptions.StrategyName, CommandLineOptions.PlayersCount, CommandLineOptions.Seed);
        var reportFullPath1 = simulationRunner.GetReportFullPath();
        var reportFullPath2 = simulationRunner2.GetReportFullPath();

        var firstReport = JsonNode.Parse(File.ReadAllText(reportFullPath1))!.AsObject();
        var secondReport = JsonNode.Parse(File.ReadAllText(reportFullPath2))!.AsObject();

        firstReport.Remove("generatedAt");
        secondReport.Remove("generatedAt");

        bool areEqual = JsonNode.DeepEquals(firstReport, secondReport);
        Assert.True(areEqual);
    }
}