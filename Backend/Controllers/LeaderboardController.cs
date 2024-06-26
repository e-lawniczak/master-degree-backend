﻿using ClothBackend.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClothBackend.Controllers
{
    [ApiController]
    [Route("/api/leaderboards/")]
    public class LeaderboardController : Controller
    {

        [HttpGet]
        [Route("get")]
        [Authorize]
        public async Task<IActionResult> GetLeaderboard()
        {
            try
            {

                var ld = new LeaderboardsDAL();
                var res = await ld.GetLeaderboardData();
                return Ok(new { Leaderboards = res });
            }
            catch (Exception ex)
            {

                return new JsonResult(ex.Message + " \n" + ex.InnerException)
                {
                    StatusCode = 500,
                };
            }
        }
    }
}
