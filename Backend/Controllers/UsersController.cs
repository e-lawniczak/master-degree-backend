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
        public JsonResult GetAllUsers()
        {
            var users = UserDAL.UserList();
            if (users == null)
                return new JsonResult("Error");
            return new JsonResult(users);
        }
    }
}
