using CashBackMerge.App.core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace CashBackMerge.App.simulation;

public record ReportModel
{
    public CampaignConfig Config { get; private set; }
    public GameSession[] Sessions { get; private set; }
    public string StrategyName { get; private set; }
    public int PlayersCount { get; private set; }
    private int _seed;
    public ReportModel(CampaignConfig config, GameSession[] sessions, string strategyName, int playersCount)
    {
        Config = config;
        Sessions = sessions;
        StrategyName = strategyName;
        PlayersCount = playersCount;
        _seed = Sessions[0].Seed;
    }
    public void GenerateReportFile()
    {
        if (Sessions == null || Sessions.Length == 0)
        {
            throw new InvalidOperationException("Невозможно сгенерировать отчет: список сессий пуст.");
        }
        var maxCashbacks = Sessions
            .Select(t => t.MaxCashback)
            .ToArray();
        var tileDistribution = maxCashbacks
            .GroupBy(cashback => cashback)
            .OrderBy(group => group.Key)
            .ToDictionary(
                group => group.Key.ToString(),
                group => group.Count()
            );
        var rewardByCashback = Config.RewardTable!
            .ToDictionary(tier => tier.Cashback, tier => tier.Reward);
        var rewards = maxCashbacks
            .Select(cashback => rewardByCashback.GetValueOrDefault(cashback, 0))
            .ToArray();
        var rewardDistribution = rewards
            .GroupBy(reward => reward)
            .OrderBy(group => group.Key)
            .ToDictionary(
                group => group.Key.ToString(),
                group => group.Count());
        var actualMaxRewardRate = (decimal)maxCashbacks
            .Count(cashback => cashback >= Config.MaxRewardCashback) / Sessions.Length;
        var status = actualMaxRewardRate <= Config.MaxRewardAllowedRate
            ? "OK"
            : "FAILED";
        var reportData = new
        {
            GeneratedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss UTC"),
            simulation = new
            {
                seed = _seed,
                PlayersCount = PlayersCount,
                strategy = StrategyName
            },
            results = new
            {
                tileDistribution,
                rewardDistribution,
                averageScore = Math.Round(Sessions.Average(s => s.Score), 2),
                maxScore = Sessions.Max(s => s.Score),
                actualMaxRewardRate = Math.Round(actualMaxRewardRate, 4),
                maxRewardAllowedRate = Config.MaxRewardAllowedRate,
                status
            },
            CampaignConfig = new
            {
                BoardSize = Config.BoardSize,
                MaxRewardAllowedRate = Config.MaxRewardAllowedRate,
                MaxRewardCashback = Config.MaxRewardCashback,
                MovesLimit = Config.MovesLimit,
                RewardTable = Config.RewardTable,
                SpawnRules = Config.SpawnRules
            },
        };
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        string jsonString = JsonSerializer.Serialize(reportData, options);
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string fileName = $"report_{StrategyName}_{timestamp}.json";
        string fullPath = Path.Combine("reports", fileName);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
        File.WriteAllText(fullPath, jsonString);

        Console.WriteLine($"[Отчет] Успешно сгенерирован: {Path.GetFullPath(fullPath)}");
    }
}
