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


        public async Task<AuthenticationResponse> LoginAsync(UserLogin request)
        {
            var eduUser = await FindByUserNameAsync(request?.UserName ?? "");

            if (eduUser == null)
                throw new Exception("Not found user with given email");
            var hash = BCrypt.Net.BCrypt.HashPassword(request?.Password, System.Configuration.ConfigurationManager.AppSettings["Salt"]);
            if (hash != eduUser.Password)
                throw new UnauthorizedAccessException("Wrong email or password");
            string jwtSecurityToken = await GenerateToken(eduUser);
            await SaveJwtToken(jwtSecurityToken, eduUser.UserId);

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
            var existing = await FindByUserNameAsync(request?.Email ?? "");

            if (existing != null)
                throw new Exception($"User {request?.UserName} already exists.");

            var hashPassword = BCrypt.Net.BCrypt.HashPassword(request?.Password, System.Configuration.ConfigurationManager.AppSettings["Salt"]);
            var user = new User
            {
                UserName = request.UserName,
                Email = request?.Email ?? "",
                Password = hashPassword
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

        private async Task SaveJwtToken( string token, int userId)
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
        private async Task<bool> CreateAsync(User user, string password)
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
            item["FirstLogin"] = false;
            item["CurrentPlaytrough"] = null;
            item["Attempts"] = 0;
            item["Deaths"] = 0;
            item["HighScore"] = 0;

            new SqlCommandBuilder(da);
            dataTable.Rows.Add(item);
            var rows =  da.Update(dataTable);
            if(rows == 0)
            {
                return false;
            }

            return true;
        }

        public async Task<User> FindByUserNameAsync(string userName)
        {
            string query = $"SELECT * FROM Users WHERE UserName = '{userName}'";
            SqlDataAdapter da = new SqlDataAdapter(query, con);
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
