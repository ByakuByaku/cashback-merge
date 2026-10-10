using CashBackMerge.App;
using CashBackMerge.App.core;
using static System.Collections.Specialized.BitVector32;

var json = """
    {
      "boardSize": 4,
      "movesLimit": 20,
      "spawnRules": [
        { "cashback": "1", "probability": "0.85" },
        { "cashback": "2", "probability": "0.15" }
      ],
      "rewardTable": [
        { "cashback": "2",  "reward": "0" },
        { "cashback": "4",  "reward": "50" },
        { "cashback": "8",  "reward": "150" },
        { "cashback": "16", "reward": "500" },
        { "cashback": "32", "reward": "2000" }
      ],
      "maxRewardCashback": "32",
      "maxRewardAllowedRate": "0.20"
    }
    """;
var path = Path.GetTempFileName();
File.WriteAllText(path, json);
var config = CampaignConfig.LoadConfig(path);
File.Delete(path);


var session = new GameSession(config);
ConsoleGame.Run(session);