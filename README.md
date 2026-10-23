# Homework 2 - Cashback Merge

Мини-игра в духе 2048 на поле 4 × 4. Одинаковые плитки кэшбэка объединяются: 1% + 1% = 2%, затем 4%, 8%, 16% и 32%. Число ходов ограничено.

## требования
- .NET 10 SDK

## Запуск
```
dotnet run --project src\CashBackMerge.App play --config fullPath --seed SEED
```
где fullPath - путь до файла с конфигом, SEED - целое число
Управление: 'W', 'A', 'S', 'D' или стрелки, 'Q' выход.
## Симуляция
```
dotnet run --project src\CashBackMerge.App simulate --config fullPath --players playersCount --strategy strategyName --seed SEED
```
где fullPath - путь до файла с конфигом, SEED - целое число, strategyName - название стратегии

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

## Пример отчета
```json
{
  "boardSize": 4,
  "movesLimit": 30,
  "spawnRules": [
    {
      "cashback": "1",
      "probability": "0.85"
    },
    {
      "cashback": "2",
      "probability": "0.15"
    }
  ],
  "rewardTable": [
    {
      "cashback": "2",
      "reward": "0"
    },
    {
      "cashback": "4",
      "reward": "50"
    },
    {
      "cashback": "8",
      "reward": "150"
    },
    {
      "cashback": "16",
      "reward": "500"
    },
    {
      "cashback": "32",
      "reward": "2000"
    }
  ],
  "maxRewardCashback": "32",
  "maxRewardAllowedRate": "0.20"
}
```

## Структура
```
src/CashBackMerge.App/core/
  Config.cs        загрузка JSON и валидация (Config, SpawnRule)
  Tile.cs          плитка, IsEmpty
  Field.cs         поле, доступ по координатам, текстовый вывод
  GameSession.cs   состояние партии, начальное состояние по seed, появление плитки
  GameStatus.cs    IN_PROGRESS, FINISHED
  MoveEngine.cs    логика ходов, слияние плиток в линиях
src/CashBackMerge.App/CLI/
  CommandLineOptions.cs создание DTO для запуска игры/симуляции
  CommandLineParser.cs обработка ввода из терминала
src/CashBackMerge.App/simulation/
  strategies/
    RandomStrategy.cs
    CornerStrategy.cs
  ReportModel.cs
  SimulationRunner.cs
src/CashBackMerge.App/modes/
  IGameMode.cs
  ConsoleGame.cs
  SimulationGame.cs
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
