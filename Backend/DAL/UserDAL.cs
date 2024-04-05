using ClothBackend.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Data;
using System;
using System.Configuration;
using System.Configuration;

namespace ClothBackend.DAL
{
    public   class UserDAL : BaseDAL
    {
        public UserDAL(IConfiguration configuration) : base(configuration)
        {
        }

        public static List<User> UserList()
        {
            string cs = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnectionString"].ConnectionString;
            SqlConnection con = new SqlConnection(cs);
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Users", con);
            DataTable dataTable = new DataTable();
            da.Fill(dataTable);
            if (dataTable.Rows.Count <= 0)
                return null;

            List<User> users = new List<User>();
            for(int i=0; i< dataTable.Rows.Count; i++)
            {
                var item = dataTable.Rows[i];
                users.Add(new User
                {
                    UserId = Convert.ToInt32(item["UserId"]),
                    UserName = Convert.ToString(item["UserName"]),
                    Password = Convert.ToString(item["Password"]),
                    Email = Convert.ToString(item["Email"]),
                    IsControlGroup = Convert.ToBoolean(item["IsControlGroup"]),
                    FirstLogin = Convert.ToBoolean(item["FirstLogin"]),
                    CurrentPlaytrough = Convert.ToInt32(item["CurrentPlaytrough"]),
                    Attempts = Convert.ToInt32(item["Attempts"]),
                    Deaths = Convert.ToInt32(item["Deaths"]),
                    HighScore = Convert.ToInt32(item["HighScore"]),

                });
            }
            return users;

        }
    }
}
