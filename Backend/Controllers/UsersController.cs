using ClothBackend.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClothBackend.Controllers
{
    [ApiController]
    [Route("/api/users")]
    public class UsersController : Controller
    {
        [HttpGet]
        [Route("all")]
        [Authorize]
        public async Task<JsonResult> GetAllUsers()
        {
            return Json("Authorized");
        }
    }
}
