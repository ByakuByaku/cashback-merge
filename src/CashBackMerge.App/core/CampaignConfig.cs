using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CashBackMerge.App.core;

public record CampaignConfig(IReadOnlyList<SpawnRule> SpawnRules,
    int MovesLimit,
    int BoardSize,
    IReadOnlyList<RewardTable>? RewardTable,
    int MaxRewardCashback,
    decimal MaxRewardAllowedRate)
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        NumberHandling = JsonNumberHandling.AllowReadingFromString
    };
    public static CampaignConfig LoadConfig(string filePath)
    {
        var json = File.ReadAllText(filePath);
        var config = JsonSerializer.Deserialize<CampaignConfig>(json, Options)
            ?? throw new InvalidOperationException("Пустой конфиг spawnRules");
        config.Validate();
        return config;
    }
    private void Validate()
    {
        if (SpawnRules.Count == 0)
        {
            throw new InvalidOperationException("Список правил пуст");
        }

        if (SpawnRules.Any(r => r.Probability is < 0 or > 1))
        {
            throw new InvalidOperationException("Вероятность вне диапазона [0, 1]");
        }

        if (Math.Abs(SpawnRules.Sum(r => r.Probability) - 1.0m) > 1e-9m)
        {
            throw new InvalidOperationException("Сумма вероятностей должна быть равна 1");
        }

        if (SpawnRules.Any(r => r.Cashback < 0))
        {
            throw new InvalidOperationException("Cashback не может быть отрицательным");
        }

        if (SpawnRules.Any(r => MovesLimit <= 0))
        {
            throw new InvalidOperationException("movesLimit не может быть отрицательным");
        }

        if (SpawnRules is null)
        {
            throw new InvalidOperationException("Нет поля spawnRules");
        }
    }
}

public record SpawnRule(int Cashback, decimal Probability);
public record RewardTable(int Cashback, int Reward);
