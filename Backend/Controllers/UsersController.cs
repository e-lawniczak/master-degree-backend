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
                    attempts = user.Attempts,
                    deaths = user.Deaths,
                    highScore = user.HighScore,
                    canNowSaveGame = user.CanNowSaveGame,

                };

                return Ok(res);
            }
            catch (Exception ex)
            {
                return new JsonResult(ex.Message + "\n" + ex.InnerException)
                {
                    StatusCode = 500,
                };
            }


        }
        
        [HttpPost]
        [Route("updateMetrics")]
        [Authorize]
        public async Task<IActionResult> UpdateMetrics(FirstLoginMetricsRequest firstLogin)
        {
            try
            {
                var ud = new UserDAL();
                var res = await ud.UpdateMetrics(firstLogin);

               

                return Ok(res);
            }
            catch (Exception ex)
            {
                return new JsonResult(ex.Message + "\n" + ex.InnerException)
                {
                    StatusCode = 500,
                };
            }


        }
        [HttpPost]
        [Route("changeMode")]
        [Authorize]
        public async Task<IActionResult> ChangeMode(ChangeMode request)
        {
            try
            {
                var ud = new UserDAL();
                var res = await ud.ChangePlayMode(request.UserId);



                return Ok(res);
            }
            catch (Exception ex)
            {
                return new JsonResult(ex.Message + "\n" + ex.InnerException)
                {
                    StatusCode = 500,
                };
            }


        }
    }
}
