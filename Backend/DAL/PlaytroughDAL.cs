using ClothBackend.DAL.Models;
using ClothBackend.Models;
using ClothBackend.Models.Checkpoint;
using ClothBackend.Models.Playtrough;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ClothBackend.DAL
{
    public class PlaytroughDAL
    {
        private readonly string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnectionString"].ConnectionString;
        private readonly SqlConnection con;

        public PlaytroughDAL()
        {
            con = new SqlConnection(connectionString);
        }
        internal async Task<InitialData> GetInitialData(int id)
        {
            UserDAL ud = new UserDAL();
            User u = await ud.FindByUserIdAsync(id);
            if (u == null)
            {
                throw new Exception("User not found");
            }
            return new InitialData
            {
                currentPlaytrough = u.CurrentPlaytrough,
                highScore = u.HighScore,
                isControlGroup = u.IsControlGroup
            };

        }
        internal async Task<bool> SaveCheckpoint(CheckpointRequest checkpoint)
        {
            bool res = await Checkpoint(checkpoint);
            return res;
        }

        internal async Task<CheckpointResponse> GetCheckpoint(int id)
        {
            var cp = await FindCheckpointById(id);
            if (cp == null) throw new Exception("Checkpoint not found");
            return cp;
        }

        internal async Task<PlaytroughResponse> GetPlaytrough(int id)
        {
            var pl = await FindPlaytroughById(id);
            if (pl == null) throw new Exception("Playtrough not found");
            return pl;
        }

        internal async Task<int> UpdatePlaytrough(PlaytroughRequest playtrough)
        {
            var pl = await FindPlaytroughById(playtrough.PlaytroughId);
            int res;
            if (pl == null)
            {
                res = await NewPlaytrough(playtrough);
            }
            else
            {
                res = await ExistingPlaytrough(playtrough);

            }
            return res;
        }

        private async Task<bool> Checkpoint(CheckpointRequest checkpoint)
        {
            if (checkpoint.PlaytroughId < 1) throw new Exception("Error saving chekpoint");

            string query = $"DELETE * FROM Checkpoints WHERE PlaytroughId = @playtroughId";
            var cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("playtroughId", checkpoint.PlaytroughId);
            cmd.ExecuteNonQuery();

            query = $"SELECT * FROM Checkpoints WHERE 0=1";
            cmd = new SqlCommand(query, con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dataTable = new DataTable();
            da.Fill(dataTable);

            var item = dataTable.Rows.Add();
            item["CheckpointId"] = checkpoint.CheckpointId;
            item["Data"] = -1;
            item["LevelNo"] = checkpoint.LevelNo;
            item["PlayerPosX"] = checkpoint.PlayerPosX;
            item["PlayerPosY"] = checkpoint.PlayerPosY;
            item["Health"] = checkpoint.Health;
            item["DefeatedEnemiesIds"] = checkpoint.DefeatedEnemiesIds;
            item["CollectedCoinsIds"] = checkpoint.CollectedCoinsIds;
            item["PlaytroughId"] = checkpoint.PlaytroughId;
            item["Date"] = DateTime.Now;

            new SqlCommandBuilder(da);
            var rows = da.Update(dataTable);
            if (rows == 0)
            {
                return false;
            }

            return true;
        }

        private async Task<int> ExistingPlaytrough(PlaytroughRequest playtrough)
        {
            string query = $"SELECT * FROM Playtroughs WHERE PlaytroughId = @playtroughId";
            var cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("playtroughId", playtrough.PlaytroughId);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dataTable = new DataTable();
            da.Fill(dataTable);

            if (dataTable.Rows.Count <= 0)
                throw new Exception("Playtrough does not exist");
            var item = dataTable.Rows[0];

            item["PlaytroughId"] = playtrough.PlaytroughId;
            item["TotalTime"] = playtrough.TotalTime;
            item["TotalPoints"] = playtrough.TotalPoints;
            item["CoinsCollected"] = playtrough.CoinsCollected;
            item["EnemiesDefeated"] = playtrough.EnemiesDefeated;
            item["PercentageProgress"] = playtrough.PercentageProgress;
            item["Deaths"] = playtrough.Deaths;
            item["TotalEnemyProxTime"] = playtrough.TotalEnemyProxTime;
            item["StandingStillTime"] = playtrough.StandingStillTime;
            item["Score"] = playtrough.Score;
            item["IsFinished"] = playtrough.IsFinished;
            item["LevelTime_1"] = playtrough.LevelTime_1;
            item["LevelPoints_1"] = playtrough.LevelPoints_1;
            item["LevelEnemies_1"] = playtrough.LevelEnemies_1;
            item["LevelCoins_1"] = playtrough.LevelCoins_1;
            item["LevelDeaths_1"] = playtrough.LevelDeaths_1;
            item["LevelEndHp_1"] = playtrough.LevelEndHp_1;
            item["LevelTime_2"] = playtrough.LevelTime_2;
            item["LevelPoints_2"] = playtrough.LevelPoints_2;
            item["LevelEnemies_2"] = playtrough.LevelEnemies_2;
            item["LevelCoins_2"] = playtrough.LevelCoins_2;
            item["LevelDeaths_2"] = playtrough.LevelDeaths_2;
            item["LevelEndHp_2"] = playtrough.LevelEndHp_2;
            item["LevelTime_3"] = playtrough.LevelTime_3;
            item["LevelPoints_3"] = playtrough.LevelPoints_3;
            item["LevelEnemies_3"] = playtrough.LevelEnemies_3;
            item["LevelCoins_3"] = playtrough.LevelCoins_3;
            item["LevelDeaths_3"] = playtrough.LevelDeaths_3;
            item["LevelEndHp_3"] = playtrough.LevelEndHp_3;
            item["UserId"] = playtrough.UserId;
            item["StartTime"] = playtrough.StartTime;
            item["EndTime"] = playtrough.EndTime;
            item["LastUpdate"] = playtrough.LastUpdate;

            new SqlCommandBuilder(da);
            var rows = da.Update(dataTable);
            if (rows == 0)
            {
                throw new Exception("Playtrough does not exist");
            }

            return playtrough.PlaytroughId;
        }

        private async Task<int> NewPlaytrough(PlaytroughRequest playtrough)
        {
            SqlDataAdapter da = new SqlDataAdapter($"SELECT * FROM Playtroughs WHERE 0 = 1", con);
            DataTable dataTable = new DataTable();
            da.Fill(dataTable);

            var item = dataTable.NewRow();


            item["TotalTime"] = playtrough.TotalTime;
            item["TotalPoints"] = playtrough.TotalPoints;
            item["CoinsCollected"] = playtrough.CoinsCollected;
            item["EnemiesDefeated"] = playtrough.EnemiesDefeated;
            item["PercentageProgress"] = playtrough.PercentageProgress;
            item["Deaths"] = playtrough.Deaths;
            item["TotalEnemyProxTime"] = playtrough.TotalEnemyProxTime;
            item["StandingStillTime"] = playtrough.StandingStillTime;
            item["Score"] = playtrough.Score;
            item["IsFinished"] = playtrough.IsFinished;
            item["LevelTime_1"] = playtrough.LevelTime_1;
            item["LevelPoints_1"] = playtrough.LevelPoints_1;
            item["LevelEnemies_1"] = playtrough.LevelEnemies_1;
            item["LevelCoins_1"] = playtrough.LevelCoins_1;
            item["LevelDeaths_1"] = playtrough.LevelDeaths_1;
            item["LevelEndHp_1"] = playtrough.LevelEndHp_1;
            item["LevelTime_2"] = playtrough.LevelTime_2;
            item["LevelPoints_2"] = playtrough.LevelPoints_2;
            item["LevelEnemies_2"] = playtrough.LevelEnemies_2;
            item["LevelCoins_2"] = playtrough.LevelCoins_2;
            item["LevelDeaths_2"] = playtrough.LevelDeaths_2;
            item["LevelEndHp_2"] = playtrough.LevelEndHp_2;
            item["LevelTime_3"] = playtrough.LevelTime_3;
            item["LevelPoints_3"] = playtrough.LevelPoints_3;
            item["LevelEnemies_3"] = playtrough.LevelEnemies_3;
            item["LevelCoins_3"] = playtrough.LevelCoins_3;
            item["LevelDeaths_3"] = playtrough.LevelDeaths_3;
            item["LevelEndHp_3"] = playtrough.LevelEndHp_3;
            item["UserId"] = playtrough.UserId;
            item["StartTime"] = DateTime.UtcNow;
            item["EndTime"] = DBNull.Value;
            item["LastUpdate"] = playtrough.LastUpdate;

            new SqlCommandBuilder(da);
            dataTable.Rows.Add(item);
            var rows = da.Update(dataTable);
            if (rows == 0)
            {
                throw new Exception("Playtrough not created");
            }

            string query = $"SELECT TOP 1 PlaytroughId FROM Playtroughs WHERE UserId = @userId ORDER By PlaytroughId desc";
            var cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("userId", playtrough.UserId);
            da = new SqlDataAdapter(cmd);
            dataTable = new DataTable();
            da.Fill(dataTable);

            return Convert.ToInt32(dataTable.Rows[0]["PlaytroughId"]);
        }

        public async Task<PlaytroughResponse> FindPlaytroughById(int pId)
        {
            if (pId < 1) return null;
            string query = $"SELECT * FROM Playtroughs WHERE PlaytroughId = @playtroughId";
            var cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("playtroughId", pId);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dataTable = new DataTable();
            da.Fill(dataTable);
            if (dataTable.Rows.Count <= 0)
                return null;
            var item = dataTable.Rows[0];
            return new PlaytroughResponse
            {
                PlaytroughId = Convert.ToInt32(item["PlaytroughId"]),
                TotalTime = Convert.ToInt32(item["TotalTime"]),
                TotalPoints = Convert.ToInt32(item["TotalPoints"]),
                CoinsCollected = Convert.ToInt32(item["CoinsCollected"]),
                EnemiesDefeated = Convert.ToInt32(item["EnemiesDefeated"]),
                PercentageProgress = Convert.ToInt32(item["PercentageProgress"]),
                Deaths = Convert.ToInt32(item["Deaths"]),
                TotalEnemyProxTime = Convert.ToInt32(item["TotalEnemyProxTime"]),
                StandingStillTime = Convert.ToInt32(item["StandingStillTime"]),
                Score = Convert.ToInt32(item["Score"]),
                IsFinished = Convert.ToBoolean(item["IsFinished"]),
                LevelTime_1 = Convert.ToInt32(item["LevelTime_1"]),
                LevelPoints_1 = Convert.ToInt32(item["LevelPoints_1"]),
                LevelEnemies_1 = Convert.ToInt32(item["LevelEnemies_1"]),
                LevelCoins_1 = Convert.ToInt32(item["LevelCoins_1"]),
                LevelDeaths_1 = Convert.ToInt32(item["LevelDeaths_1"]),
                LevelEndHp_1 = Convert.ToInt32(item["LevelEndHp_1"]),
                LevelTime_2 = Convert.ToInt32(item["LevelTime_2"]),
                LevelPoints_2 = Convert.ToInt32(item["LevelPoints_2"]),
                LevelEnemies_2 = Convert.ToInt32(item["LevelEnemies_2"]),
                LevelCoins_2 = Convert.ToInt32(item["LevelCoins_2"]),
                LevelDeaths_2 = Convert.ToInt32(item["LevelDeaths_2"]),
                LevelEndHp_2 = Convert.ToInt32(item["LevelEndHp_2"]),
                LevelTime_3 = Convert.ToInt32(item["LevelTime_3"]),
                LevelPoints_3 = Convert.ToInt32(item["LevelPoints_3"]),
                LevelEnemies_3 = Convert.ToInt32(item["LevelEnemies_3"]),
                LevelCoins_3 = Convert.ToInt32(item["LevelCoins_3"]),
                LevelDeaths_3 = Convert.ToInt32(item["LevelDeaths_3"]),
                LevelEndHp_3 = Convert.ToInt32(item["LevelEndHp_3"]),
                UserId = Convert.ToInt32(item["UserId"]),
                StartTime = item["StartTime"] != DBNull.Value ? (DateTime)(item["StartTime"]) : DateTime.UtcNow,
                EndTime = item["EndTime"] != DBNull.Value ? (DateTime)(item["EndTime"]) : null,
                LastUpdate = item["LastUpdate"] != DBNull.Value ? (DateTime)(item["LastUpdate"]) : null

            };
        }
        public async Task<CheckpointResponse> FindCheckpointById(int cId)
        {
            if (cId < 1) return null;
            string query = $"SELECT * FROM Checkpoints WHERE PlaytroughId = @checkpointId";
            var cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("checkpointId", cId);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dataTable = new DataTable();
            da.Fill(dataTable);
            if (dataTable.Rows.Count <= 0)
                return null;
            var item = dataTable.Rows[0];
            return new CheckpointResponse
            {
                CheckpointId = Convert.ToInt32(item["CheckpointId"]),
                Data = Convert.ToInt32(item["Data"]),
                LevelNo = Convert.ToInt32(item["LevelNo"]),
                PlayerPosX = Convert.ToInt32(item["PlayerPosX"]),
                PlayerPosY = Convert.ToInt32(item["PlayerPosY"]),
                Health = Convert.ToInt32(item["Health"]),
                DefeatedEnemiesIds = Convert.ToString(item["DefeatedEnemiesIds"]).Split().Select(s => Int32.Parse(s)).ToList(),
                CollectedCoinsIds = Convert.ToString(item["CollectedCoinsIds"]).Split().Select(s => Int32.Parse(s)).ToList(),
                PlaytroughId = Convert.ToInt32(item["PlaytroughId"]),
                Date = (DateTime)(item["Date"]),

            };
        }


    }
}
