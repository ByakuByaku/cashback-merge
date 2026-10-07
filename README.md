# Homework 2 - Cashback Merge

Мини-игра в духе 2048 на поле 4 × 4. Одинаковые плитки кэшбэка объединяются: 1% + 1% = 2%, затем 4%, 8%, 16% и 32%. Число ходов ограничено.

## требования
- .NET 10 SDK

## Запуск
```
dotnet run --project src/CashBackMerge.App
```

## Тесты
```
dotnet run
```

## Конфиг игры
```json
{
  "movesLimit": 20,
  "spawnRules": [
    { "cashback": "1", "probability": "0.85" },
    { "cashback": "2", "probability": "0.15" }
  ]
}
```
Некорректный конфиг отклоняется исключением `InvalidOperationException`
с описанием причины.

## Структура
```
src/CashBackMerge.App/core/
  Config.cs        загрузка JSON и валидация (Config, SpawnRule)
  Tile.cs          плитка, IsEmpty
  Field.cs         поле, доступ по координатам, текстовый вывод
  GameSession.cs   состояние партии, начальное состояние по seed, появление плитки
  GameStatus.cs    IN_PROGRESS, FINISHED
```
За выбор значения новой плитки отвечает `GameSession.PickCashback`,
за выбор клетки `GameSession.SpawnTile`. Чтобы поменять правило появления,
достаточно править `spawnRules` в JSON, лимит ходов меняется полем `movesLimit`.

## Пример вывода поля

```
  1   .   .   .
  .   .   2   .
  .   .   .   .
  .   .   .   .
```
`.` обозначает пустую клетку.
