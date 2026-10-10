using CashBackMerge.App.core;
using System.Reflection;

namespace CashBackMerge.Tests.Mandatory.Mr2;

/// <summary>Обязательные тесты: движок ходов — что считается ходом и что после него происходит.</summary>
public class MoveEngineTest
{
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
            return new GameSession(config, seed: 102);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact(DisplayName = "Невалидный ход не меняет поле, не тратит ход и не создаёт плитку")]
    public void InvalidMoveChangesNothing()
    {
        var session = NewSession(20);
        for (int r = 0; r < session.GameField.Tiles.GetLength(0); r++)
        {
            for (int c = 0; c < session.GameField.Tiles.GetLength(1); c++)
            {
                session.GameField.SetTile(r, c, new Tile(0));
            }
        }
        session.GameField.SetTile(0, 3, new Tile(2));
        var startField = session.GameField.Tiles;
        var move = MoveEngine.ApplyMove(session, MoveDirection.RIGHT);
        var endField = session.GameField.Tiles;
        Assert.False(move.Changed);
        for (int r = 0; r < startField.GetLength(0); r++)
        {
            for (int c = 0; c < startField.GetLength(1); c++)
            {
                Assert.Equal(startField[r, c].Cashback, endField[r, c].Cashback);
            }
        }
    }

    [Fact(DisplayName = "После валидного хода появляется ровно одна новая плитка")]
    public void ValidMoveSpawnsExactlyOneTile()
    {
        var session = NewSession(20);
        for (int r = 0; r < session.GameField.Tiles.GetLength(0); r++)
        {
            for (int c = 0; c < session.GameField.Tiles.GetLength(1); c++)
            {
                session.GameField.SetTile(r, c, new Tile(0));
            }
        }
        session.GameField.SetTile(0, 0, new Tile(1));
        var startField = session.GameField.Tiles;
        MoveEngine.ApplyMove(session, MoveDirection.RIGHT);
        var endField = session.GameField.Tiles;
        int startTilesCount = 0;
        int endTilesCount = 0;
        for (int r = 0; r < startField.GetLength(0); r++)
        {
            for (int c = 0; c < startField.GetLength(1); c++)
            {
                if (startField[r, c].Cashback != 0)
                {
                    startTilesCount++;
                }
                if (endField[r, c].Cashback != 0)
                {
                    endTilesCount++;
                }
            }
        }
        Assert.Equal(1, Math.Abs(startTilesCount - endTilesCount));
    }

    [Fact(DisplayName = "После N валидных ходов игра завершена")]
    public void GameEndsAfterMovesLimit()
    {
        var session = NewSession(5);
        var directions = new MoveDirection[] { MoveDirection.DOWN, MoveDirection.LEFT, MoveDirection.RIGHT, MoveDirection.UP };
        var rnd = new Random();
        while (session.Status == GameStatus.IN_PROGRESS)
        {
            MoveEngine.ApplyMove(session, directions[rnd.Next(0, 3)]);
        }
        Assert.Equal(GameStatus.FINISHED, session.Status);

    }

    [Fact(DisplayName = "Достижение плитки 32% не заканчивает игру — доигрываем до лимита ходов")]
    public void ReachingMaxTileDoesNotEndGame()
    {
        var session = NewSession(5);
        session.GameField.SetTile(0, 0, new Tile(32));
        Exception? exception = Record.Exception(() => MoveEngine.ApplyMove(session, MoveDirection.RIGHT));
        Assert.Null(exception);
    }

    [Fact(DisplayName = "Score считается по правилам и растёт только при слияниях")]
    public void ScoreIsCalculatedOnMerges()
    {
        var session = NewSession(5);
        for (int r = 0; r < session.GameField.Tiles.GetLength(0); r++)
        {
            for (int c = 0; c < session.GameField.Tiles.GetLength(1); c++)
            {
                session.GameField.SetTile(r, c, new Tile(0));
            }
        }
        session.GameField.SetTile(0, 3, new Tile(2));

        var move = MoveEngine.ApplyMove(session, MoveDirection.LEFT);
        Assert.True(move.Changed);
        Assert.Equal(0, move.ScoreGained);
        Assert.Equal(0, session.Score);

        session.GameField.SetTile(0, 2, new Tile(2));
        var move2 = MoveEngine.ApplyMove(session, MoveDirection.LEFT);
        var rewardTable = session.GameConfig.RewardTable.ToDictionary(r => r.Cashback, r => r.Reward);
        var expectedScore = rewardTable[4];
        Assert.True(move2.Changed);
        Assert.Equal(expectedScore, move2.ScoreGained);
        Assert.Equal(expectedScore, session.Score);
    }
}