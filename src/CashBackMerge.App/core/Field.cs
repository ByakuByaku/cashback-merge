using System;
using System.Collections.Generic;
using System.Text;
using CashBackMerge.App.core;

namespace CashBackMerge.App.core;

public class Field
{
    private Tile[,] _tiles;
    
    public Tile[,]? Tiles
    {
        get
        {
            return _tiles.Clone() as Tile[,];
        }
        
    }
    public int Size { get; private set; }

    public Field(int size)
    {
        Size = size;
        _tiles = new Tile[size, size];
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                _tiles[x, y] = new Tile();
            }
        }
    }
    public void SetTile(int x, int y, Tile tile)
    {
        if (x < 0 || x >= Size || y < 0 || y >= Size)
        {
            throw new   ("Координаты плитки за границей игрового поля");
        }
        _tiles[x, y] = tile;
    }
    public (int, int)[] GetEmptyTiles()
    {
        var emptyTiles = new List<(int, int)>();
        for (int x = 0; x < Size; x++)
        {
            for (int y = 0; y < Size; y++)
            {
                if (_tiles[x, y].Cashback == 0)
                {
                    emptyTiles.Add((x,y));
                }
            }
        }
        return emptyTiles.ToArray();
    }
    public override string ToString()
    {
        var rows = new List<string>();
        for (int y = 0; y < Size; y++)
        {
            var row = new List<string>();
            for (int x = 0; x < Size; x++)
            {
                if (_tiles[x, y].Cashback == 0)
                {
                    row.Add(".");
                }
                else
                    row.Add(_tiles[x, y].Cashback.ToString());
            }
            rows.Add(string.Join(" ", row));
        }
        return string.Join("\n", rows);
    }
    public Tile GetTile(int x, int y)
    {
        if (x < 0 || x >= Size || y < 0 || y >= Size)
        {
            throw new ArgumentOutOfRangeException("Координаты плитки за границей игрового поля");
        }
        return _tiles[x, y];
    }
}
