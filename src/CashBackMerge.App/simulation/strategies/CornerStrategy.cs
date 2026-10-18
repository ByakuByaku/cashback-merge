using CashBackMerge.App.core;
using System;
using System.Collections.Generic;
using System.Text;

namespace CashBackMerge.App.simulation.strategies;

public class CornerStrategy : IStrategy
{
    public string Name => "CornerStrategy";
    private readonly MoveDirection[] _priority = {
        MoveDirection.DOWN,
        MoveDirection.RIGHT,
        MoveDirection.LEFT,
        MoveDirection.UP
    };
    private int _attemptIndex = 0;

    public MoveDirection GetNextMove()
    {
        var direction = _priority[_attemptIndex % _priority.Length];
        _attemptIndex++;
        return direction;
    }
}
