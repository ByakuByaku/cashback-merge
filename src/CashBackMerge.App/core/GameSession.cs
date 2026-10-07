using System;
using System.Collections.Generic;
using System.Text;

namespace CashBackMerge.App.core;

public class GameSession
{
    private int _startTailsCount = 2;
    public GameStatus Status { get; private set; }
    public Field GameField { get; private set; }
    public Config GameConfig { get; private set; }
    public GameSession(Config config, int size, int? seed = null)
    {
        GameConfig = config;
        Status = GameStatus.IN_PROGRESS;
        GameField = new Field(size);
        var rnd = seed.HasValue ? new Random(seed.Value) : new Random();
        for (int i = 0; i < _startTailsCount; i++)
        {
            SpawnTile(rnd);
        }
    }
    public void SpawnTile(Random rnd)
    {
        var emptyPositions = GameField.GetEmptyTiles();
        var (x, y) = emptyPositions[rnd.Next(emptyPositions.Length)];
        var cashback = PickCashback(rnd);
        GameField.SetTile(x, y, new Tile(cashback));
    }
    private int PickCashback(Random rnd)
    {
        var roll = (decimal)rnd.NextDouble();
        var cumulative = 0m;

        foreach (var rule in GameConfig.SpawnRules)
        {
            cumulative += rule.Probability;
            if (roll < cumulative)
            {
                return rule.Cashback;
            }
        }

        return GameConfig.SpawnRules[^1].Cashback;
    }

}
