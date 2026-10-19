using CashBackMerge.App.core;
using System;
using System.Collections.Generic;
using System.Text;

namespace CashBackMerge.App.simulation.strategies;

public class RandomStrategy : IStrategy
{
    public string Name => "RandomStrategy";
    private MoveDirection[] _moveDirections = { MoveDirection.DOWN, MoveDirection.LEFT, MoveDirection.RIGHT, MoveDirection.UP };
    private Random _random;
    public RandomStrategy(Random random)
    {
        _random = random;
    }
    public MoveDirection GetNextMove()
    {
        return _moveDirections[_random.Next(0, 4)];
    }
}
