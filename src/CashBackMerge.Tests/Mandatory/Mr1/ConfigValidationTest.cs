using CashBackMerge.App.core;
namespace CashBackMerge.Tests.Mandatory.Mr1;

/// <summary>Обязательные тесты: конфиг кампании.</summary>
public class ConfigValidationTest
{
    private static CampaignConfig Load(string json)
    {
        var path = Path.GetTempFileName();
        try
        {
            File.WriteAllText(path, json);
            return CampaignConfig.LoadConfig(path);
        }
        finally
        {
            File.Delete(path);
        }
    }
    [Fact(DisplayName = "Валидный конфиг принимается")]
    public void ValidConfigIsAccepted()
    {
        var config = Load("""
            {
              "boardSize": 4,
              "movesLimit": 20,
              "spawnRules": [
                { "cashback": "1", "probability": "0.85" },
                { "cashback": "2", "probability": "0.15" }
              ]
            }
            """);
        Assert.Equal(20, config.MovesLimit);
        Assert.Equal(2, config.SpawnRules.Count);
        Assert.Equal(new SpawnRule(1, 0.85m), config.SpawnRules[0]);
        Assert.Equal(new SpawnRule(2, 0.15m), config.SpawnRules[1]);
    }

    [Theory(DisplayName = "Сумма вероятностей spawn-правил не равна 1 — конфиг отклоняется")]
    [InlineData("0.85", "0.1")]
    [InlineData("0.7", "0.5")]
    [InlineData("0.5", "0.5000001")]
    public void ProbabilitiesNotSummingToOneAreRejected(params string[] probabilities)
    {
        var json = $$"""
            {
              "boardSize": 4,
              "movesLimit": 20,
              "spawnRules": [
                { "cashback": "1", "probability": "{{probabilities[0]}}" },
                { "cashback": "2", "probability": "{{probabilities[1]}}" }
              ]
            }
            """;
        Assert.Throws<InvalidOperationException>(() => Load(json));
    }

    [Fact(DisplayName = "movesLimit <= 0 — конфиг отклоняется")]
    public void NonPositiveMovesLimitIsRejected()
    {
        var json = ("""
            {
              "boardSize": 4,
              "movesLimit": 0,
              "spawnRules": [
                { "cashback": "1", "probability": "0.85" },
                { "cashback": "2", "probability": "0.15" }
              ]
            }
            """);
        Assert.Throws<InvalidOperationException>(() => Load(json));
    }

    [Fact(DisplayName = "Пустой список spawn-правил — конфиг отклоняется")]
    public void EmptySpawnRulesAreRejected()
    {
        var json = ("""
            {
              "movesLimit": 0,
              "spawnRules": []
            }
            """);
        Assert.Throws<InvalidOperationException>(() => Load(json));
    }
}