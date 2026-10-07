using System;
using System.Collections.Generic;
using System.Text;

namespace CashBackMerge.App.core;

public class Tile
{
    public int Cashback { get; private set; }
    public bool IsEmpty => Cashback == 0;

    public Tile(int cashback = 0)
    {
        Cashback = cashback;
    }
    public override string ToString()
    {
        return Cashback.ToString();
    }
}
