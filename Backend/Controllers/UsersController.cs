using ClothBackend.DAL;
using ClothBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClothBackend.Controllers
{
    [ApiController]
    [Route("/api/users")]
    public class UsersController : Controller
    {
        [HttpGet]
        [Route("getUserData/{playerId}")]
        [Authorize]
        public async Task<IActionResult> GetAllUsers(int playerId)
        {
            try
            {
                var ud = new UserDAL();
                var user = await ud.GetPlayerStats(playerId);

                UserDataResponse res = new UserDataResponse
                {
                    userName = user.UserName,
                    email = user.Email,
                    isControlGroup = user.IsControlGroup,
                    firstLogin = user.FirstLogin,
                    currentPlaytrough = user.CurrentPlaytrough,
                    attempts = user.Attempts,
                    deaths = user.Deaths,
                    highScore = user.HighScore
                };

                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }
    }
}
