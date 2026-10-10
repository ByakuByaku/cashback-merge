using CashBackMerge.App.core;
using System;
using System.Collections.Generic;
using System.Text;

namespace CashBackMerge.App;

public static class ConsoleGame
{
    public static void Run(GameSession session)
    {
        while (session.Status == GameStatus.IN_PROGRESS)
        {
            Console.WriteLine();
            Console.WriteLine(session.GameField.ToString());
            Console.WriteLine($"Осталось ходов: {session.GameConfig.MovesLimit - session.MovesMade}");
            var key = Console.ReadKey(true).Key;
            if (key == ConsoleKey.Q)
            {
                Console.WriteLine("Выход из игры.");
                break;
            }
            var direction = GetMoveDirection(key);
            if (direction is null)
            {
                Console.WriteLine("Некорректный ввод. Используйте W/A/S/D или стрелки для хода, Q для выхода.");
                continue;
            }

            var result = MoveEngine.ApplyMove(session, direction.Value);
            if (result.Changed)
            {
                Console.WriteLine($"Получено: {result.ScoreGained} очков.");
            }
            else
            {
                Console.WriteLine("Ход не изменил состояние поля.");
            }
        }
        Console.WriteLine();
        Console.WriteLine("Финальное поле:");
        Console.WriteLine(session.GameField.ToString());
        Console.WriteLine();
        Console.WriteLine($"Партия окончена. Финальный счет: {session.Score}. Ваш кешбек: {session.MaxCashback}%");
    }

    private static MoveDirection? GetMoveDirection(ConsoleKey key)
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
