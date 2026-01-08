using DTBWebServer.GameLogic;
using DTBWebServer.GameLogic.JiXianTiaoZhan;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace DTBWebServer.DataBase
{
    public static class DbInitializer
    {
        public const string LimitChallengeTableName = "JiXianTiaoZhanInfoList";

        public static async Task Initialize(WorldDBContext dbContext)
        {
            dbContext.Database.EnsureCreated();
            dbContext.SaveChanges();
            // var sql = dbContext.Database.GenerateCreateScript();

            string sqlRaw = @$" CREATE TABLE If Not Exists `{LimitChallengeTableName}` (
                  `nUserId` varchar(95) CHARACTER SET utf8mb4 NOT NULL,
                  `nActivityId` int unsigned NOT NULL,
                  `nTime` int unsigned NOT NULL,
                  `nScore` int unsigned NOT NULL,
                  `nRank` int unsigned NOT NULL,
                  CONSTRAINT `PK_{LimitChallengeTableName}` PRIMARY KEY (`nUserId`)
            ) CHARACTER SET=utf8mb4; ";
            dbContext.Database.ExecuteSqlRaw(sqlRaw);

            //这里注释掉，因为
            await ReImportTianTiData(dbContext);
        }

        public static async Task<int> DeleteAllData(WorldDBContext dbContext)
        {
            string sqlRaw = @$"DELETE FROM {LimitChallengeTableName};";
            return await dbContext.Database.ExecuteSqlRawAsync(sqlRaw);
        }

        public static async Task ReImportTianTiData(WorldDBContext dbContext)
        {
            int nActivityId = JiXianTiaoZhanModel.Instance.GetCurrentActivityId();
            if (nActivityId <= 0)
            {
                return;
            }
            
            List<DbUser> mListDbUser = new List<DbUser>();
            dbContext.t_user.ToList().ForEach(t =>
            {
                if (t.open_id.Length > 20)
                {
                    mListDbUser.Add(t);
                }
            });

            List<DbUser> mRandomDbUserList = new List<DbUser>();
            for (int i = 0; i < 100; i++)
            {
                if (mListDbUser.Count > 0)
                {
                    int nRandomIndex = RandomUtility.Random(0, mListDbUser.Count - 1);
                    mRandomDbUserList.Add(mListDbUser[nRandomIndex]);
                    mListDbUser.RemoveAt(nRandomIndex);
                }
                else
                {
                    break;
                }
            }

            foreach (var t in mRandomDbUserList)
            {
                string uId = t.open_id;
                bool bCreate = false;
                DbJiXianTiaoZhanInfo mInfo = await dbContext.JiXianTiaoZhanInfoList.FindAsync(uId);
                if (mInfo == null)
                {
                    mInfo = new DbJiXianTiaoZhanInfo();
                    bCreate = true;
                }

                mInfo.nUserId = uId;
                mInfo.nActivityId = (uint)nActivityId;
                mInfo.nTime = (uint)RandomUtility.Random(0, 200);
                mInfo.nScore = (uint)(mInfo.nTime * RandomUtility.Random(100, 300));
                mInfo.nRank = 0;

                if (bCreate)
                {
                    await dbContext.JiXianTiaoZhanInfoList.AddAsync(mInfo);
                }
                else
                {
                    dbContext.JiXianTiaoZhanInfoList.Update(mInfo);
                }
            };

            await dbContext.SaveChangesAsync();
        }

        public static async Task<JsonResult> FindJiXianTiaoZhanUserData(WorldDBContext dbContext, string uId)
        {
            DbJiXianTiaoZhanInfo mJiXianTiaoZhanInfo = await dbContext.JiXianTiaoZhanInfoList.FindAsync(uId);
            return new JsonResult(mJiXianTiaoZhanInfo);
        }

        public static async Task<JsonResult> FindTianTiUserData(WorldDBContext dbContext, string uId)
        {
            DbUser mInfo = await dbContext.t_user.FindAsync(uId);
            return new JsonResult(mInfo);
        }


    }
}