using DTBWebServer.DataBase;
using DTBWebServer.GameLogic;
using DTBWebServer.GameLogic.JiXianTiaoZhan;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DTBWebServer.Controllers.JiXianTiaoZhan
{
    [Route($"{GameConst.routePrefix}/[controller]")]
    [ApiController]
    [Authorize]
    public class JiXianTiaoZhanController : ControllerBase
    {
        public readonly WorldDBContext worldDBContext;
        public readonly int nActivityId = -1;

        public JiXianTiaoZhanController(WorldDBContext worldDBContext)
        {
            this.worldDBContext = worldDBContext;
            this.nActivityId = JiXianTiaoZhanModel.Instance.GetCurrentActivityId();
        }

        [HttpPost]
        [Route("GetActivityInfo")]
        public async Task<JsonResult> GetActivityInfo()
        {
            var result = await JiXianTiaoZhanActivity.GetActivityInfo(this);
            return result;
        }

        [HttpPost]
        [Route("GetRankList")]
        public async Task<JsonResult> GetRankList()
        {
            var result = await JiXianTiaoZhanActivity.GetRankList(this);
            return result;
        }

        [HttpPost]
        [Route("UploadChengji")]
        public async Task<JsonResult> UploadChengji()
        {
            var result = await JiXianTiaoZhanActivity.UploadChengji(this);
            return result;
        }
    }
}

