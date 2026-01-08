using DTBWebServer.DataBase;
using DTBWebServer.GameLogic;
using DTBWebServer.GameLogic.HotSong;
using DTBWebServer.GameLogic.JiXianTiaoZhan;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DTBWebServer.Controllers.HotSong
{
    [Route($"{GameConst.routePrefix}/[controller]")]
    [ApiController]
    [Authorize]
    public class HotSongController : ControllerBase
    {
        public readonly WorldDBContext worldDBContext = null;

        public HotSongController(WorldDBContext worldDBContext, ILogger<HotSongController> logger)
        {
            this.worldDBContext = worldDBContext;
        }

        [HttpGet]
        [Route("GetHotSongList")]
        public async Task<JsonResult> GetRankList(int rankCount)
        {
            return await HotSongGameLogic.GetRankList(this, rankCount);
        }

    }
}

