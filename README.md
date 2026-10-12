# Homework 2 - Cashback Merge

Мини-игра в духе 2048 на поле 4 × 4. Одинаковые плитки кэшбэка объединяются: 1% + 1% = 2%, затем 4%, 8%, 16% и 32%. Число ходов ограничено.

## требования
- .NET 10 SDK

## Запуск
```
dotnet run --project src/CashBackMerge.App
dotnet run --project src/CashBackMerge.App -- SEED
```
Управление: 'W', 'A', 'S', 'D' или стрелки, 'Q' выход.

## Тесты
```
dotnet test
```

## Конфиг игры
```json
{
  "boardSize": 4,
  "movesLimit": 20,
  "spawnRules": [
    { "cashback": "1", "probability": "0.85" },
    { "cashback": "2", "probability": "0.15" }
  ],
  "maxRewardCashback": "32",
  "maxRewardAllowedRate": "0.20"
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
  MoveEngine.cs    логика ходов, слияние плиток в линиях
ConsoleGame.cs    консольная версия игры
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
