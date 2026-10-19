using CashBackMerge.App.core;
using CashBackMerge.App.simulation.strategies;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;

namespace CashBackMerge.App.simulation;

public class SimulationRunner
{
    private readonly CampaignConfig _config;

    public SimulationRunner(CampaignConfig config)
    {
        _config = config;
    }

    public void RunBatchSinulation(string strategyName, int playersCount, int? initialSeed = null)
    {
        int baseSeed = initialSeed ?? new Random().Next(1, 100000);
        var sessions = new List<GameSession>();
        Console.WriteLine($"=== СТАРТ ПАКЕТНОЙ СИМУЛЯЦИИ ===");
        Console.WriteLine($"Конфигурация: Размер поля {_config.BoardSize}x{_config.BoardSize}, Лимит ходов: {_config.MovesLimit}");
        Console.WriteLine($"Количество игроков (повторений) на стратегию: {playersCount}");
        Console.WriteLine($"Базовый Seed симуляции: {baseSeed}\n");
        for (int i = 0; i < playersCount; i++)
        {
            int currentSeed = baseSeed + i;
            var session = new GameSession(_config, currentSeed);
            var strategy = ResolveStrategy(strategyName, currentSeed);
            while (session.Status == GameStatus.IN_PROGRESS)
            {
                var move = strategy.GetNextMove();
                var result = MoveEngine.ApplyMove(session, move);
                
            }
            sessions.Add(session);
        }
        var report = new ReportModel(_config, sessions.ToArray(), strategyName, playersCount);
        report.GenerateReportFile();
        Console.WriteLine($"Симуляция Завершена\n");
    }
    public IStrategy ResolveStrategy(string strategyName, int seed)
    {
        switch (strategyName)
        {
            case "RandomStrategy":
                return new RandomStrategy(new Random(seed));
            case "CornerStrategy":
                return new CornerStrategy();
            default:
                throw new InvalidOperationException("Некорректное название стратегии");
        }
    }
}
