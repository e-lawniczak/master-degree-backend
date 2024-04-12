using ClothBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClothBackend.DAL
{
    public class LeaderboardsDAL
    {
        private readonly string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnectionString"].ConnectionString;
        private readonly SqlConnection con;

        public LeaderboardsDAL()
        {
            con = new SqlConnection(connectionString);
        }
        public async Task<List<Leaderboards>> GetLeaderboardData()
        {
            List<Leaderboards> leaderboards = GenerateLeaderboard();
            return leaderboards;
        }

        private List<Leaderboards> GenerateLeaderboard()
        {
            string query = $"SELECT TOP 10 * FROM Users ORDER BY HighScore desc";
            var cmd = new SqlCommand(query, con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dataTable = new DataTable();
            da.Fill(dataTable);
            if(dataTable.Rows.Count  == 0 )
            {
                throw new Exception("No users found");
            }
            List<Leaderboards> res = new List<Leaderboards>();

            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                var item = dataTable.Rows[i];
                res.Add(new Leaderboards
                {
                    UserName = Convert.ToString(item["UserName"]),
                    HighScore = Convert.ToInt32(item["HighScore"]),
                });
            }
            return res;
        }
    }
}
