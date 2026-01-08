using DTBWebServer.DataBase;
using DTBWebServer.GameLogic;
using DTBWebServer.GameLogic.JiXianTiaoZhan;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace DTBWebServer.Controllers.GMCommand
{
    [Route($"{GameConst.routePrefix}/[controller]")]
    [ApiController]
    [Authorize]
    public class GMCommandController : ControllerBase
    {
        public ILogger<GMCommandController> mLogManager = null;
        public WorldDBContext worldDBContext = null;
        public DbSet<DbJiXianTiaoZhanInfo> mDbSet;
        public int nActivityId = -1;
        private const string GMPassword = @"***weqwefg789464132qjwepqjwQWIRUEASFDLApoejfowjepofjap&&&@#$%***";

        public GMCommandController(WorldDBContext worldDBContext, ILogger<GMCommandController> logger)
        {
            this.mLogManager = logger;
            this.worldDBContext = worldDBContext;
            this.mDbSet = worldDBContext.JiXianTiaoZhanInfoList;
            this.nActivityId = JiXianTiaoZhanModel.Instance.GetCurrentActivityId();
        }

/*      [HttpPost]
        [Route("JiXianTiaoZhanActivity_Json")]
        public async Task<JsonResult> ReceiveLimitChallengeJsonFile(IFormFile file)
        {
            try
            {
                string tempDir = Path.Combine(AppContext.BaseDirectory, "Temp/");
                if (!Directory.Exists(tempDir))
                {
                    Directory.CreateDirectory(tempDir);
                }
                
                string tempPath = Path.Combine(tempDir, "JiXianChallengeData.json");
                using (var targetStream = System.IO.File.Open(tempPath, FileMode.OpenOrCreate))
                {
                    await file.CopyToAsync(targetStream);
                }

                if (JiXianTiaoZhanModel.Instance.CheckJsonConfigFileError(tempPath))
                {
                    string saveToPath = Path.Combine(AppContext.BaseDirectory, "ExcelData/JiXianChallengeData.json");
                    System.IO.File.Copy(tempPath, saveToPath, true);
                    System.IO.File.Delete(tempPath);
                    JiXianTiaoZhanModel.Instance.Init();
                    return new JsonResult("True");
                }
                else
                {
                    return new JsonResult("False");
                }
            }
            catch (Exception ex)
            {
                return new JsonResult("Exception：" + ex.Message);
            }
        }*/
        
        [HttpGet]
        [Route("JiXianTiaoZhanActivity_Json_FormCDN")]
        public async Task<JsonResult> ReceiveLimitChallengeJsonFileFormCDN(string managerPassword, string cdnJsonUrl)
        {
            try
            {
                if (@managerPassword != @GMPassword)
                {
                    return new JsonResult("False");
                }

                using (HttpClient client = new HttpClient())
                {
                    string fileContent = await client.GetStringAsync(cdnJsonUrl);
                    string tempDir = Path.Combine(AppContext.BaseDirectory, "Temp/");
                    if (!Directory.Exists(tempDir))
                    {
                        Directory.CreateDirectory(tempDir);
                    }

                    string tempPath = Path.Combine(tempDir, "JiXianChallengeData.json");
                    await System.IO.File.WriteAllTextAsync(tempPath, fileContent);

                    if (JiXianTiaoZhanModel.Instance.CheckJsonConfigFileError(tempPath))
                    {
                        string saveToPath = Path.Combine(AppContext.BaseDirectory, "ExcelData/JiXianChallengeData.json");
                        System.IO.File.Copy(tempPath, saveToPath, true);
                        System.IO.File.Delete(tempPath);
                        JiXianTiaoZhanModel.Instance.Init();
                        return new JsonResult("True");
                    }
                    else
                    {
                        return new JsonResult("False");
                    }
                }
            }
            catch (Exception ex)
            {
                return new JsonResult("Exception：" + ex.Message);
            }
        }

        [HttpGet]
        [Route("JiXianTiaoZhanActivity_DbInitFromTianTi")]
        public async Task<JsonResult> JiXianTiaoZhanActivity_DbInitFromTianTi(string managerPassword)
        {
            try
            {
                if (@managerPassword != @GMPassword)
                {
                    return new JsonResult("False");
                }

                await DbInitializer.ReImportTianTiData(worldDBContext);
                return new JsonResult("True");
            }
            catch (Exception ex)
            {
                return new JsonResult("Exception：" + ex.Message);
            }
        }

        [HttpGet]
        [Route("JiXianTiaoZhanActivity_FindJiXianTiaoZhanUserData")]
        public async Task<JsonResult> JiXianTiaoZhanActivity_FindJiXianTiaoZhanUserData(string managerPassword, string uId)
        {
            try
            {
                if (@managerPassword != @GMPassword)
                {
                    return new JsonResult("False");
                }

                return await DbInitializer.FindJiXianTiaoZhanUserData(worldDBContext, uId);
            }
            catch (Exception ex)
            {
                return new JsonResult("Exception：" + ex.Message);
            }
        }

        [HttpGet]
        [Route("JiXianTiaoZhanActivity_FindTianTiUserData")]
        public async Task<JsonResult> JiXianTiaoZhanActivity_FindTianTiUserData(string managerPassword, string uId)
        {
            try
            {
                if (@managerPassword != @GMPassword)
                {
                    return new JsonResult("False");
                }

                return await DbInitializer.FindTianTiUserData(worldDBContext, uId);
            }
            catch (Exception ex)
            {
                return new JsonResult("Exception：" + ex.Message);
            }
        }

        [HttpGet]
        [Route("JiXianTiaoZhanActivity_GetServerLogContent")]
        public async Task<ActionResult> JiXianTiaoZhanActivity_GetServerLogContent(string managerPassword)
        {
            try
            {
                if (@managerPassword != @GMPassword)
                {
                    ContentResult mResult = new ContentResult();
                    mResult.Content = "GM 密码错误!";
                    return mResult;
                }

                string filePath = AppContext.BaseDirectory + "Temp/NLogfile.log";
                if (!System.IO.File.Exists(filePath))
                {
                    ContentResult mResult = new ContentResult();
                    mResult.Content = "一切Ok, 还未产生日志!";
                    return mResult;
                }

                string fileContent = "";
                using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    fileContent = await reader.ReadToEndAsync();
                }

                ContentResult mResult1 = new ContentResult();
                mResult1.Content = fileContent;
                return mResult1;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex.Message);
                return null;
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("JiXianTiaoZhanActivity_GetServerLogFile")]
        public async Task<FileStreamResult> JiXianTiaoZhanActivity_GetServerLogFile(string managerPassword)
        {
            try
            {
                if (@managerPassword != @GMPassword)
                {
                    return null;
                }

                string filePath = AppContext.BaseDirectory + "Temp/NLogfile.log";
                if (!System.IO.File.Exists(filePath))
                {
                    return null;
                }

                using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    return new FileStreamResult(stream, "text/plain; charset=utf-8");
                }

            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex.Message);
                return null;
            }
        }


        /*        [HttpGet]
                [Route("JiXianTiaoZhanActivity_DeleteAllDbData")]
                public async Task<JsonResult> JiXianTiaoZhanActivity_DeleteAllDbData(string managerPassword)
                {
                    try
                    {
                        if (@managerPassword != @GMPassword)
                        {
                            return new JsonResult("False");
                        }

                        await DbInitializer.DeleteAllData(worldDBContext);
                        return new JsonResult("True");
                    }
                    catch (Exception ex)
                    {
                        return new JsonResult("Exception：" + ex.Message);
                    }
                }*/

    }
}
