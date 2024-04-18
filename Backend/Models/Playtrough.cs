using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClothBackend.Models.Playtrough
{
    public class PlaytroughResponse
    {
        public int PlaytroughId { get; set; }
        public float? TotalTime { get; set; }
        public int? TotalPoints { get; set; }
        public int? CoinsCollected { get; set; }
        public int? EnemiesDefeated { get; set; }
        public float? PercentageProgress { get; set; }
        public int? Deaths { get; set; }
        public float TotalEnemyProxTime { get; set; }
        public float StandingStillTime { get; set; }
        public int Score { get; set; }
        public bool IsFinished { get; set; }
        public float? LevelTime_1 { get; set; }
        public int? LevelPoints_1 { get; set; }
        public int? LevelEnemies_1 { get; set; }
        public int? LevelCoins_1 { get; set; }
        public int? LevelDeaths_1 { get; set; }
        public int? LevelEndHp_1 { get; set; }
        public float? LevelTime_2 { get; set; }
        public int? LevelPoints_2 { get; set; }
        public int? LevelEnemies_2 { get; set; }
        public int? LevelCoins_2 { get; set; }
        public int? LevelDeaths_2 { get; set; }
        public int? LevelEndHp_2 { get; set; }
        public float? LevelTime_3 { get; set; }
        public int? LevelPoints_3 { get; set; }
        public int? LevelEnemies_3 { get; set; }
        public int? LevelCoins_3 { get; set; }
        public int? LevelDeaths_3 { get; set; }
        public int? LevelEndHp_3 { get; set; }
        public int UserId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime? LastUpdate { get; set; }


    }
    public class PlaytroughRequest
    {
        public int PlaytroughId { get; set; }
        public float? TotalTime { get; set; }
        public int? TotalPoints { get; set; }
        public int? CoinsCollected { get; set; }
        public int? EnemiesDefeated { get; set; }
        public float? PercentageProgress { get; set; }
        public int? Deaths { get; set; }
        public float TotalEnemyProxTime { get; set; }
        public float StandingStillTime { get; set; }
        public int Score { get; set; }
        public bool IsFinished { get; set; }
        public float? LevelTime_1 { get; set; }
        public int? LevelPoints_1 { get; set; }
        public int? LevelEnemies_1 { get; set; }
        public int? LevelCoins_1 { get; set; }
        public int? LevelDeaths_1 { get; set; }
        public int? LevelEndHp_1 { get; set; }
        public bool LevelFinished_1 { get; set; }
        public float? LevelTime_2 { get; set; }
        public int? LevelPoints_2 { get; set; }
        public int? LevelEnemies_2 { get; set; }
        public int? LevelCoins_2 { get; set; }
        public int? LevelDeaths_2 { get; set; }
        public int? LevelEndHp_2 { get; set; }
        public bool LevelFinished_2 { get; set; }
        public float? LevelTime_3 { get; set; }
        public int? LevelPoints_3 { get; set; }
        public int? LevelEnemies_3 { get; set; }
        public int? LevelCoins_3 { get; set; }
        public int? LevelDeaths_3 { get; set; }
        public int? LevelEndHp_3 { get; set; }
        public bool LevelFinished_3 { get; set; }
        public int UserId { get; set; }
        public long StartTime { get; set; }
        public long EndTime { get; set; }
        public long LastUpdate { get; set; }
        public string DefeatedEnemiesIds { get; set; }
        public string CollectedCoinsIds { get; set; }

    }
}
