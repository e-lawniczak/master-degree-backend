using ClothBackend.DAL;
using ClothBackend.DAL.Models;
using ClothBackend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PizzeriaAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ClothBackend.Controllers
{
    [ApiController]
    [Route("/api/auth")]
    public class AuthenticationController : Controller
    {
        [HttpPost]
        [Route("credentials")]
        public async Task<JsonResult> Login(UserLogin request)
        {
            try
            {
                var ud = new UserDAL();
                bool isRegistered = await ud.CheckUser(request.UserName);
                AuthenticationResponse res;
                if (isRegistered)
                {
                    res = await ud.LoginAsync(request);

                }
                else
                {
                    res = await ud.RegisterAsync(request);
                }
                return Json(res);
            }
            catch (Exception ex)
            {
                return new JsonResult(new { Msg=ex.Message, inner=ex.InnerException });
            }


        }


    }
}
