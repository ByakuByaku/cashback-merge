using System;
using System.Collections.Generic;
using System.Text;

namespace CashBackMerge.App.core;

public class GameSession
{
    private const int StartTilesCount = 2;
    private readonly bool _spawnTiles;
    private readonly Random _random;
    public int Seed { get; private set; }
    public GameStatus Status { get; private set; }
    public Field GameField { get; private set; }
    public CampaignConfig GameConfig { get; private set; }
    public int Score { get; private set; }
    public int MovesMade { get; private set; }
    public int MaxCashback => GameField.Tiles.Cast<Tile>().Max(t => t.Cashback);

    public GameSession(CampaignConfig config, int? seed = null)
        : this(config, true, seed) { }
    public GameSession(CampaignConfig config, bool spawnTiles, int? seed = null)
    {
        GameConfig = config;
        Status = GameStatus.IN_PROGRESS;
        GameField = new Field(GameConfig.BoardSize);
        _random = seed.HasValue ? new Random(seed.Value) : new Random();
        _spawnTiles = spawnTiles;
        Seed = seed ?? 0;
        if (_spawnTiles)
        {
            for (int i = 0; i < StartTilesCount; i++)
            {
                SpawnTile(_random);
            }
        }
    }
    internal void SpawnTile(Random rnd)
    {
        var emptyPositions = GameField.GetEmptyTiles();
        var (x, y) = emptyPositions[rnd.Next(emptyPositions.Length)];
        if (emptyPositions.Length == 0)
        {
            throw new InvalidOperationException("Нет свободных клеток для новой плитки");
        }

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
    internal void RecordMove(int gainedScore)
    {
        Score += gainedScore;
        MovesMade++;
        if (_spawnTiles)
        {
            SpawnTile(_random);
        }
        if (MovesMade >= GameConfig.MovesLimit || !GameField.HasAvailableMoves())
        {
            Status = GameStatus.FINISHED;
        }
    }
}
