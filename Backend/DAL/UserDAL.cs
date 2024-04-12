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
using Microsoft.AspNetCore.Identity;
using PizzeriaAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using ClothBackend.Models;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace ClothBackend.DAL
{
    public class UserDAL
    {

        private readonly string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnectionString"].ConnectionString;
        private readonly SqlConnection con;

        public UserDAL()
        {
            con = new SqlConnection(connectionString);
        }

        #region Methods
        public async Task<bool> UpdateHighScore(int userId, int score)
        {
            var update = await UpdateHS(userId, score);
            return update;
        }
        public async Task<User> GetPlayerStats(int playerId)
        {
            var user = await FindByUserIdAsync(playerId);
            return user;
        }
        public async Task<bool> UpdateMetrics(FirstLoginMetricsRequest data)
        {
            var user = await FindByUserIdAsync(data.UserId);
            if (user == null)
                throw new Exception("User not found");

            bool succeded = await UpdateMetricsQuery(data, user);

            return succeded;
        }



        public async Task<AuthenticationResponse> LoginAsync(UserLogin request)
        {
            if (request.UserName == null || request.UserName.Length <= 0)
                throw new Exception("Please enter valid username");

            if (request.Password == null || request.Password.Length < 8)
                throw new Exception("Please enter valid password. At least 8 characters long");

            if (request == null)
                throw new Exception("Request is null");

            //if (request.Agree1 == false || request.Agree2 == false || request.Agree3 == false)
            //    throw new Exception($"Accept all agreements");

            var eduUser = await FindByUserNameAsync(request.UserName);

            if (eduUser == null)
                throw new Exception("Not found user with given user name");
            var hash = BCrypt.Net.BCrypt.HashPassword(request?.Password, System.Configuration.ConfigurationManager.AppSettings["Salt"]);
            if (hash != eduUser.Password)
                throw new UnauthorizedAccessException("Wrong user name or password");
            string jwtSecurityToken = await GenerateToken(eduUser);
            //await SaveJwtToken(jwtSecurityToken, eduUser.UserId);

            AuthenticationResponse response = new AuthenticationResponse
            {
                Id = eduUser.UserId,
                Token = jwtSecurityToken,
                Email = eduUser.Email,
                UserName = eduUser.UserName,
                IsFirstLogin = eduUser.FirstLogin
            };

            return response;
        }



        private async Task<string> GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user?.UserName??""),
            new Claim("uid", user?.UserId.ToString()??""),
            };

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(System.Configuration.ConfigurationManager.AppSettings["Key"] ?? ""));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);


            var tokenDescriptior = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["DurationInMinutes"] ?? "120")),
                Issuer = System.Configuration.ConfigurationManager.AppSettings["Issuer"],
                Audience = System.Configuration.ConfigurationManager.AppSettings["Audience"],
                SigningCredentials = signingCredentials
            };
            var token = tokenHandler.CreateToken(tokenDescriptior);


            return tokenHandler.WriteToken(token);
        }

        internal async Task<bool> CheckUser(string userName)
        {
            var user = await FindByUserNameAsync(userName);
            if (user == null)
            {
                return false;
            }
            return true;
        }

        public async Task<AuthenticationResponse> RegisterAsync(UserLogin request)
        {
            if (request == null)
                throw new Exception("Request is null");

            if (request.UserName == null || request.UserName.Length <= 0)
                throw new Exception("Please enter valid username");

            if (request.Password == null || request.Password.Length < 8)
                throw new Exception("Please enter valid password. At least 8 characters long");

            if (request == null)
                throw new Exception("Request is null");

            //if (request.Agree1 == false || request.Agree2 == false || request.Agree3 == false)
            //    throw new Exception($"Accept all agreements");

            var existing = await FindByUserNameAsync(request?.Email ?? "");

            if (existing != null)
                throw new Exception($"User {request?.UserName} already exists.");


            var hashPassword = BCrypt.Net.BCrypt.HashPassword(request?.Password, System.Configuration.ConfigurationManager.AppSettings["Salt"]);
            var user = new UserLogin
            {
                UserName = request.UserName,
                Email = request?.Email ?? "",
                Password = hashPassword,
                Agree1 = request?.Agree1 ?? false,
                Agree2 = request?.Agree2 ?? false,
                Agree3 = request?.Agree3 ?? false

            };

            var result = await CreateAsync(user, request?.Password ?? "");

            if (result)
            {
                var u = await LoginAsync(request);
                return u;
            }
            else
            {
                throw new Exception($"error while creating account");
            }

        }

        #endregion
        #region DB Queries
        private async Task<bool> UpdateHS(int userId, int newScore)
        {
            string query = $"SELECT * FROM Users WHERE UserId = @userId";
            var cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("userId", userId);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dataTable = new DataTable();
            da.Fill(dataTable);

            if (dataTable.Rows.Count <= 0)
                throw new Exception("User does not exist");
            var item = dataTable.Rows[0];

            if (Convert.ToInt32(item["HighScore"]) < newScore)
            {
                da.UpdateCommand = new SqlCommandBuilder(da).GetUpdateCommand();
                item["HighScore"] = newScore;


                var rows = da.Update(dataTable);
                if (rows != 1)
                {
                    throw new Exception("Something went wrong");
                }
            }


            return true;
        }
        private async Task<bool> UpdateMetricsQuery(FirstLoginMetricsRequest data, User user)
        {
            string query = $"SELECT * FROM Metrics WHERE 1 = 0";
            var cmd = new SqlCommand(query, con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dataTable = new DataTable();
            da.Fill(dataTable);


            var item = dataTable.NewRow();
            item["Country"] = data.Country;
            item["Age"] = Convert.ToInt32(data.Age);
            item["Gender"] = data.Gender;
            item["GamesExperience"] = data.GamesExperience;
            item["Education"] = data.Education;
            item["DemographicBackground"] = data.DemographicBackground;
            item["UserId"] = user.UserId;

            new SqlCommandBuilder(da);
            dataTable.Rows.Add(item);
            var rows = da.Update(dataTable);


            if (rows == 0)
                throw new Exception("Metrics not saved");

            query = $"SELECT * FROM Users WHERE UserId = @userId";
            cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("userId", user.UserId);
            da = new SqlDataAdapter(cmd);
            dataTable = new DataTable();
            da.Fill(dataTable);

            if (rows == 0)
                throw new Exception("User not found");

            item = dataTable.Rows[0];
            item["FirstLogin"] = 0;

            new SqlCommandBuilder(da);
            rows = da.Update(dataTable);

            return rows > 0;
        }
        private async Task SaveJwtToken(string token, int userId)
        {
            SqlDataAdapter da = new SqlDataAdapter($"SELECT * FROM Tokens WHERE 0 = 1", con);
            DataTable dataTable = new DataTable();
            da.Fill(dataTable);
            Random rnd = new Random();
            int num = rnd.Next();

            var item = dataTable.NewRow();
            //item["UserId"] = "";
            item["UserId"] = userId;
            item["Token"] = token;
            item["Expires"] = DateTime.UtcNow.AddMinutes(Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["DurationInMinutes"] ?? "120"));

            new SqlCommandBuilder(da);
            dataTable.Rows.Add(item);
            var rows = da.Update(dataTable);

        }
        private async Task<bool> CreateAsync(UserLogin user, string password)
        {
            SqlDataAdapter da = new SqlDataAdapter($"SELECT * FROM Users WHERE 0 = 1", con);
            DataTable dataTable = new DataTable();
            da.Fill(dataTable);
            Random rnd = new Random();
            int num = rnd.Next();

            var item = dataTable.NewRow();
            //item["UserId"] = "";
            item["UserName"] = user.UserName;
            item["Password"] = user.Password;
            item["Email"] = user?.Email ?? "";
            item["IsControlGroup"] = num % 2 == 0;
            item["FirstLogin"] = true;
            item["CurrentPlaytrough"] = DBNull.Value;
            item["Attempts"] = 0;
            item["Deaths"] = 0;
            item["HighScore"] = 0;
            //item["Agreement1"] = user.Agree1;
            //item["Agreement2"] = user.Agree2;
            //item["Agreement3"] = user.Agree3;

            new SqlCommandBuilder(da);
            dataTable.Rows.Add(item);
            var rows = da.Update(dataTable);
            if (rows == 0)
            {
                return false;
            }

            return true;
        }

        public async Task<User> FindByUserNameAsync(string userName)
        {
            string query = $"SELECT * FROM Users WHERE UserName = @userName";
            string escapedName = Regex.Escape(userName);
            var cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("userName", escapedName);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dataTable = new DataTable();
            da.Fill(dataTable);
            if (dataTable.Rows.Count <= 0)
                return null;
            var item = dataTable.Rows[0];
            return new User
            {
                UserId = Convert.ToInt32(item["UserId"]),
                UserName = Convert.ToString(item["UserName"]),
                Password = Convert.ToString(item["Password"]),
                Email = Convert.ToString(item["Email"]),
                IsControlGroup = Convert.ToBoolean(item["IsControlGroup"]),
                FirstLogin = Convert.ToBoolean(item["FirstLogin"]),
                CurrentPlaytrough = DBNull.Value.Equals(item["CurrentPlaytrough"]) ? null : Convert.ToInt32(item["CurrentPlaytrough"]),
                Attempts = Convert.ToInt32(item["Attempts"]),
                Deaths = Convert.ToInt32(item["Deaths"]),
                HighScore = Convert.ToInt32(item["HighScore"]),
            };
        }
        public async Task<User> FindByUserIdAsync(int userId)
        {
            string query = $"SELECT * FROM Users WHERE UserId = @userId";
            var cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("userId", userId);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dataTable = new DataTable();
            da.Fill(dataTable);
            if (dataTable.Rows.Count <= 0)
                return null;
            var item = dataTable.Rows[0];
            return new User
            {
                UserId = Convert.ToInt32(item["UserId"]),
                UserName = Convert.ToString(item["UserName"]),
                Password = Convert.ToString(item["Password"]),
                Email = Convert.ToString(item["Email"]),
                IsControlGroup = Convert.ToBoolean(item["IsControlGroup"]),
                FirstLogin = Convert.ToBoolean(item["FirstLogin"]),
                CurrentPlaytrough = DBNull.Value.Equals(item["CurrentPlaytrough"]) ? null : Convert.ToInt32(item["CurrentPlaytrough"]),
                Attempts = Convert.ToInt32(item["Attempts"]),
                Deaths = Convert.ToInt32(item["Deaths"]),
                HighScore = Convert.ToInt32(item["HighScore"]),
            };
        }
        #endregion
    }
}
