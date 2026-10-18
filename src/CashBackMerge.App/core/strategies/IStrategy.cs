using System;
using System.Collections.Generic;
using System.Text;

namespace CashBackMerge.App.core.strategies;

public interface IStrategy
{
    public string Name { get; }
    MoveDirection GetNextMove();

}
