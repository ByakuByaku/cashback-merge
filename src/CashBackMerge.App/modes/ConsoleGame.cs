using CashBackMerge.App.core;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Collections.Specialized.BitVector32;

namespace CashBackMerge.App.modes;

public class ConsoleGame : IGameMode
{
    private GameSession _session;
    public ConsoleGame(GameSession session)
    {
        _session = session;
    }

    public void Run()
    {
        while (_session.Status == GameStatus.IN_PROGRESS)
        {
            Console.WriteLine();
            Console.WriteLine(_session.GameField.ToString());
            Console.WriteLine($"Осталось ходов: {_session.GameConfig.MovesLimit - _session.MovesMade}");
            var key = Console.ReadKey(true).Key;
            if (key == ConsoleKey.Q)
            {
                Console.WriteLine("Выход из игры.");
                break;
            }
            MoveDirection? direction = GetMoveDirection(key);
            if (direction is null)
            {
                Console.WriteLine("Некорректный ввод. Используйте W/A/S/D или стрелки для хода, Q для выхода.");
                continue;
            }

            var result = MoveEngine.ApplyMove(_session, direction.Value);
            if (!result.Changed)
            {
                Console.WriteLine("Ход не изменил состояние поля.");
            }

        }
        Console.WriteLine();
        Console.WriteLine("Финальное поле:");
        Console.WriteLine(_session.GameField.ToString());
        Console.WriteLine();
        Console.WriteLine($"Партия окончена. Финальный счет: {_session.Score}. Ваш кешбек: {_session.MaxCashback}%");
    }
    private MoveDirection? GetMoveDirection(ConsoleKey key)
    {
        return key switch
        {
            ConsoleKey.W or ConsoleKey.UpArrow => MoveDirection.UP,
            ConsoleKey.S or ConsoleKey.DownArrow => MoveDirection.DOWN,
            ConsoleKey.A or ConsoleKey.LeftArrow => MoveDirection.LEFT,
            ConsoleKey.D or ConsoleKey.RightArrow => MoveDirection.RIGHT,
            _ => null
        };
    }
}
