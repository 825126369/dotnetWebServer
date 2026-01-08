using GameLogic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ServerConfigHelpr.Controllers
{
    [Route("api")]
    [ApiController]
    public class SendConfigController : ControllerBase
    {
        [HttpPost]
        [Route("发送极限挑战Json文件")]
        public async Task<JsonResult> SendLimitChallengeJsonFile(string tokenUrl = null, string apiUrl = null)
        {
            string uId = "1106";
            string password = "123321456654789987963852741";

            if (tokenUrl == null)
            {
                tokenUrl = $"http://192.168.2.127/JiXianTiaoZhan/Authenticate?uId={uId}&pwd={password}";
            }
            else
            {
                tokenUrl = $"{tokenUrl}?uId={uId}&pwd={password}";
            }

            if (apiUrl == null)
            {
                apiUrl = "http://192.168.2.127/JiXianTiaoZhan/JiXianTiaoZhanActivity_Json";
            }

            string filePath = "ExcelData/JiXianChallengeData.json";
            await HttpHeler.Instance.SendFileToServer(tokenUrl, apiUrl, filePath);
            return new JsonResult(true);
        }
    }
}
