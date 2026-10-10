using CashBackMerge.App.core;

namespace CashBackMerge.Tests.Mandatory.Mr2;

/// <summary>Обязательные тесты: правила слияния плиток.</summary>
public class MergeRulesTest
{
    private const int TargetIndex = 0;

    private static GameSession NewSession(int movesLimit)
    {
        var path = Path.GetTempFileName();
        try
        {
            File.WriteAllText(path, $$"""
                {
                  "boardSize": 4,
                  "movesLimit": {{movesLimit}},
                  "spawnRules": [
                    { "cashback": "1", "probability": "0.85" },
                    { "cashback": "2", "probability": "0.15" }
                  ],
                  "rewardTable": [
                    { "cashback": "2",  "reward": "0" },
                    { "cashback": "4",  "reward": "50" },
                    { "cashback": "8",  "reward": "150" },
                    { "cashback": "16", "reward": "500" },
                    { "cashback": "32", "reward": "2000" }
                  ],
                  "maxRewardCashback": "32",
                  "maxRewardAllowedRate": "0.20"
                }
                """);
            var config = CampaignConfig.LoadConfig(path);
            return new GameSession(config, false, seed: 27);
        }
        finally
        {
            File.Delete(path);
        }
    }
    [Theory(DisplayName = "Сдвиг влево: строка до -> после")]
    [InlineData("1,1,.,.", "2,.,.,.")]
    [InlineData("1,1,1,.", "2,1,.,.")]
    [InlineData("1,1,1,1", "2,2,.,.")]
    [InlineData("2,2,4,4", "4,8,.,.")]

    public void RowShiftsLeftAndMerges(string before, string expected)
    {
        var session = NewSession(20);
        int[] beforeInt = before.Split(',').Select(s => s == "." ? 0 : int.Parse(s)).ToArray();
        int[] expectedInt = expected.Split(',').Select(s => s == "." ? 0 : int.Parse(s)).ToArray();
        for (int i = 0; i < 4; i++)
        {
            session.GameField.SetTile(0, i, new Tile(beforeInt[i]));
        }
        var moveResult = MoveEngine.ApplyMove(session, MoveDirection.LEFT);
        Assert.True(moveResult.Changed);
        for (int i = 0; i < 4; i++)
        {
            Assert.Equal(expectedInt[i], session.GameField.Tiles[0, i].Cashback);
        }
    }

    [Theory(DisplayName = "Одна плитка не сливается дважды за ход: [1,1,2,.] -> [2,2,.,.]")]
    [InlineData("1,1,2,.", ".,.,2,2")]
    [InlineData(".,2,2,4", ".,.,4,4")]
    [InlineData("1,1,1,1", ".,.,2,2")]
    public void TileDoesNotMergeTwiceInOneMove(string before, string expected)
    {
        var session = NewSession(6);
        int[] beforeInt = before.Split(',').Select(s => s == "." ? 0 : int.Parse(s)).ToArray();
        int[] expectedInt = expected.Split(',').Select(s => s == "." ? 0 : int.Parse(s)).ToArray();
        for (int i = 0; i < 4; i++)
        {
            session.GameField.SetTile(0, i, new Tile(beforeInt[i]));
        }
        Assert.True(MoveEngine.ApplyMove(session, MoveDirection.RIGHT).Changed);
        for (int i = 0; i < 4; i++)
        {
            Assert.Equal(expectedInt[i], session.GameField.Tiles[0, i].Cashback);
        }


    }

    [Theory(DisplayName = "Слияние работает во всех четырёх направлениях")]
    [InlineData(MoveDirection.LEFT, "1,1,.,.", "2,.,.,.")]
    [InlineData(MoveDirection.RIGHT, ".,.,1,1", ".,.,.,2")]
    [InlineData(MoveDirection.UP, "1,1,.,.", "2,.,.,.")]
    [InlineData(MoveDirection.DOWN, ".,.,1,1", ".,.,.,2")]
    public void MergeWorksInAllDirections(MoveDirection direction, string before, string expected)
    {
        var session = NewSession(6);
        int[] beforeInt = before.Split(',').Select(s => s == "." ? 0 : int.Parse(s)).ToArray();
        int[] expectedInt = expected.Split(',').Select(s => s == "." ? 0 : int.Parse(s)).ToArray();
        for (int i = 0; i < 4; i++)
        {
            if (direction == MoveDirection.LEFT || direction == MoveDirection.RIGHT)
            {
                session.GameField.SetTile(0, i, new Tile(beforeInt[i]));
            }
            else
            {
                session.GameField.SetTile(i, 0, new Tile(beforeInt[i]));
            }
        }
        Assert.True(MoveEngine.ApplyMove(session, direction).Changed);
        for (int i = 0; i < 4; i++)
        {
            if (direction == MoveDirection.LEFT || direction == MoveDirection.RIGHT)
            {
                Assert.Equal(expectedInt[i], session.GameField.Tiles[0, i].Cashback);
            }
            else
            {
                Assert.Equal(expectedInt[i], session.GameField.Tiles[i, 0].Cashback);
            }
        }
    }
}
