using CashBackMerge.App.core;

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
              "movesLimit": 20,
              "spawnRules": [
                { "cashback": "1", "probability": "0.85" },
                { "cashback": "2", "probability": "0.15" }
              ]
            }
            """);
        var config = Config.LoadConfig(path);
        var session1 = new GameSession(config, 4, seed: 42);
        var session2 = new GameSession(config, 4, seed: 42);
        Assert.Equal(session1.GameField.ToString(), session2.GameField.ToString());
        File.Delete(path);
    }

    [Fact(DisplayName = "Одинаковый конфиг, seed и стратегия дают одинаковый отчёт симуляции",
          Skip = "MR3: реализуй тест и удали эту строку")]
    public void SameInputsProduceSameSimulationReport()
    {
        Assert.Fail("Тест не реализован");
    }
}