using CashBackMerge.App.core;
using System;
using System.Collections.Generic;
using System.Text;

namespace CashBackMerge.App.simulation.strategies;

public interface IStrategy
{
    public string Name { get; }
    MoveDirection GetNextMove();

}
