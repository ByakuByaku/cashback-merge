using CashBackMerge.App.CLI;
using CashBackMerge.App.core;
using CashBackMerge.App.simulation;
using System.Text.Json;
using System.Xml.Linq;

namespace CashBackMerge.Tests.Mandatory.Mr3;

/// <summary>Обязательные тесты: награды, симуляция и отчёт.</summary>
public class RewardAndSimulationTest
{
    public CommandLineOptions CommandLineOptions = new CommandLineOptions($"simulate --config {Path.Combine(
    AppContext.BaseDirectory,
    "configs",
    "CampaignConfig.json")} --players 1 --strategy CornerStrategy --seed 5555")
    {
        ConfigPath = Path.Combine(AppContext.BaseDirectory,"configs", "CampaignConfig.json")
    };
    public string GenerateSimulation()
    {
        var config = CampaignConfig.LoadConfig(CommandLineOptions.ConfigPath);
        var simulationRunner = new SimulationRunner(config);
        simulationRunner.RunBatchSinulation(CommandLineOptions.StrategyName, CommandLineOptions.PlayersCount, CommandLineOptions.Seed);
        return simulationRunner.GetReportFullPath();
    }
    [Fact(DisplayName = "Максимальная плитка 16% даёт награду соответствующего tier")]
    public void RewardMatchesHighestTile()
    {
        var config = CampaignConfig.LoadConfig(CommandLineOptions.ConfigPath);
        var session = new GameSession(config, false, CommandLineOptions.Seed);
        for (int i = 0; i<session.GameConfig.BoardSize; i++)
        {
            for (int j = 0; j < session.GameConfig.BoardSize; j++)
            {
                session.GameField.SetTile(i, j, new Tile(0));
            }
        }
        session.GameField.SetTile(0,0, new Tile(16));
        var report = new ReportModel(config, [session], CommandLineOptions.StrategyName, CommandLineOptions.PlayersCount);
        var reportPath = report.GenerateReportFile();

        using var json = JsonDocument.Parse(File.ReadAllText(reportPath));
        var rewards = json.RootElement
            .GetProperty("results")
            .GetProperty("rewardDistribution");
        Assert.Equal(1, rewards.GetProperty("500").GetInt32());
    }

    [Fact(DisplayName = "Weighted random учитывает вероятности из конфига")]
    public void SpawnRespectsConfiguredProbabilities()
    {
        var config = CampaignConfig.LoadConfig(CommandLineOptions.ConfigPath);
        var counts = config.SpawnRules.ToDictionary(rule => rule.Cashback, _ => 0);
        var totalTiles = 0;
        const int sessionCount = 10000;

        for (int seed = 1; seed <= sessionCount; seed++)
        {
            var session = new GameSession(config, seed);
            foreach(var tile in session.GameField.Tiles)
            {
                if (!tile.IsEmpty)
                {
                    counts[tile.Cashback]++;
                    totalTiles++;
                }
            }
        }
        foreach (var rule in config.SpawnRules)
        {
            var actualRate = (decimal)counts[rule.Cashback] / totalTiles;

            Assert.InRange(actualRate, rule.Probability - 0.02m, rule.Probability + 0.02m);
        }
    }

    [Fact(DisplayName = "Отчёт содержит распределение максимальных плиток и наград")]
    public void ReportContainsDistributions()
    {
        using var json = JsonDocument.Parse(File.ReadAllText(GenerateSimulation()));
        var tileDistribution = json.RootElement
            .GetProperty("results")
            .GetProperty("tileDistribution");
        var rewardDistribution = json.RootElement
            .GetProperty("results")
            .GetProperty("rewardDistribution");
        Assert.NotEqual(JsonValueKind.Null, tileDistribution.ValueKind);
        Assert.NotEqual(JsonValueKind.Null, rewardDistribution.ValueKind);
    }

    [Fact(DisplayName = "Отчёт содержит долю игроков с максимальной наградой")]
    public void ReportContainsMaxRewardRate()
    {
        using var json = JsonDocument.Parse(File.ReadAllText(GenerateSimulation()));
        var actualMaxRewardRate = json.RootElement
            .GetProperty("results")
            .GetProperty("actualMaxRewardRate");
        Assert.NotEqual(JsonValueKind.Null, actualMaxRewardRate.ValueKind);
    }

    [Fact(DisplayName = "Статус OK, если actualMaxRewardRate <= maxRewardAllowedRate, иначе FAILED")]
    public void ReportStatusReflectsBusinessConstraint()
    {
        using var json = JsonDocument.Parse(File.ReadAllText(GenerateSimulation()));
        var actualMaxRewardRate = json.RootElement
            .GetProperty("results")
            .GetProperty("actualMaxRewardRate")
            .GetDecimal();
        var maxRewardAllowedRate = json.RootElement
            .GetProperty("campaignConfig")
            .GetProperty("maxRewardAllowedRate")
            .GetDecimal();
        var status = json.RootElement
            .GetProperty("results")
            .GetProperty("status")
            .GetString();
        if (actualMaxRewardRate <= maxRewardAllowedRate)
        {
            Assert.Equal("OK", status);
        }
        else
        {
            Assert.Equal("FAILED", status);
        }


    }
}