using ClothBackend.DAL;
using ClothBackend.DAL.Models;
using ClothBackend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PizzeriaAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
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
        public async Task<IActionResult> Login(UserLogin request)
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
                };
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Msg = ex.Message, Inner = ex.InnerException});
            }


        }


    }
}
