using ClothBackend.DAL;
using ClothBackend.Models;
using ClothBackend.Models.Checkpoint;
using ClothBackend.Models.Playtrough;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClothBackend.Controllers
{
    [ApiController]
    [Route("/api/playtrough/")]
    public class PlaytroughController : Controller
    {


        [HttpGet]
        [Route("getInitial/{id}")]
        [Authorize]
        public async Task<IActionResult> GetInitialData(int id)
        {
            try
            {
                var pd = new PlaytroughDAL();
                InitialData res = await pd.GetInitialData(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("getPlaytrough/{id}")]
        [Authorize]
        public async Task<IActionResult> GetPlaytrough(int id)
        {
            try
            {
                var pd = new PlaytroughDAL();
                PlaytroughResponse res = await pd.GetPlaytrough(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("updatePlaytrough")]
        [Authorize]
        public async Task<IActionResult> UpdatePlaytrough(PlaytroughRequest playtrough)
        {
            try
            {
                var pd = new PlaytroughDAL();
                int res = await pd.UpdatePlaytrough(playtrough);
                if (res < 1)
                {
                    return BadRequest("Something went wrong.");
                }

                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("getCheckpoint/{id}")]
        [Authorize]
        public async Task<IActionResult> GetCheckpoint(int id)
        {
            try
            {
                var pd = new PlaytroughDAL();
                CheckpointResponse res = await pd.GetCheckpoint(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("saveCheckpoint/{id}")]
        [Authorize]
        public async Task<IActionResult> SaveCheckpoint(CheckpointRequest checkpoint)
        {
            try
            {
                var pd = new PlaytroughDAL();
                bool res = await pd.SaveCheckpoint(checkpoint);
                if (!res) BadRequest("Something went wrong");
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
