using System;
using System.Collections.Generic;
using System.Text;

namespace CashBackMerge.App.core;

public record MoveResult(GameStatus GameStatus, int ScoreGained, bool Changed);
public static class MoveEngine
{
    public static MoveResult ApplyMove(GameSession state, MoveDirection direction)
    {
        if (state.Status == GameStatus.FINISHED)
        {
            throw new InvalidOperationException("Партия завершена");
        }

        var (changed, merged) = MoveTiles(state.GameField, direction);
        if (!changed)
        {
            return new MoveResult(state.Status, 0, false);
        }
        var scoreGained = CalculateScore(merged, state.GameConfig.RewardTable);
        state.RecordMove(scoreGained);
        return new MoveResult(state.Status, scoreGained, changed);
    }
    private static int CalculateScore(List<int> merged, IReadOnlyList<RewardTable>? rewardTable)
    {
        if (rewardTable is null)
        {
            return 0;
        }

        var rewards = rewardTable.ToDictionary(r => r.Cashback, r => r.Reward);
        return merged.Sum(tile => rewards.GetValueOrDefault(tile, 0));
    }
    private static (bool, List<int>) MoveTiles(Field gameField, MoveDirection direction)
    {
        var rowLength = gameField.Tiles.GetLength(0);
        var allMerged = new List<int>();
        var changed = false;
        for (int k = 0; k < rowLength; k++)
        {
            List<int> row = new List<int>();
            for (int l = 0; l < rowLength; l++)
            {
                switch (direction)
                {
                    case MoveDirection.UP:
                        row.Add(gameField.Tiles[rowLength - l - 1, k].Cashback);
                        break;
                    case MoveDirection.DOWN:
                        row.Add(gameField.Tiles[l, k].Cashback);
                        break;
                    case MoveDirection.LEFT:
                        row.Add(gameField.Tiles[k, rowLength - l - 1].Cashback);
                        break;
                    case MoveDirection.RIGHT:
                        row.Add(gameField.Tiles[k, l].Cashback);
                        break;
                }
            }
            (var result, var merged) = MergeRow(row);
            if (!result.SequenceEqual(row))
            {
                changed = true;
            }

            allMerged.AddRange(merged);
            for (int l = 0; l < rowLength; l++)
            {
                switch (direction)
                {
                    case MoveDirection.UP:
                        gameField.SetTile(rowLength - l - 1, k, new Tile(result[l]));
                        break;
                    case MoveDirection.DOWN:
                        gameField.SetTile(l, k, new Tile(result[l]));
                        break;
                    case MoveDirection.LEFT:
                        gameField.SetTile(k, rowLength - l - 1, new Tile(result[l]));
                        break;
                    case MoveDirection.RIGHT:
                        gameField.SetTile(k, l, new Tile(result[l]));
                        break;
                }
            }

        }
        return (changed, allMerged);

    }
    private static (List<int> Row, List<int> Merged) MergeRow(List<int> row)
    {
        List<int> mergedRow = new List<int>();
        var reversedRow = new List<int>(row);
        reversedRow.Reverse();
        var rowWithoutZeros = reversedRow.Where(x => x != 0).ToList();
        var merged = new List<int>();
        for (var i = 0; i < rowWithoutZeros.Count; i++)
        {
            if (i + 1 < rowWithoutZeros.Count && rowWithoutZeros[i] == rowWithoutZeros[i + 1])
            {
                mergedRow.Add(rowWithoutZeros[i] * 2);
                merged.Add(rowWithoutZeros[i] * 2);
                i++;
            }
            else
            {
                mergedRow.Add(rowWithoutZeros[i]);
            }
        }
        while (mergedRow.Count < row.Count)
        {
            mergedRow.Add(0);
        }

        mergedRow.Reverse();
        return (mergedRow, merged);
    }
}
