using DTBWebServer.Controllers.JiXianTiaoZhan;
using DTBWebServer.DataBase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Buffers;

namespace DTBWebServer.GameLogic.JiXianTiaoZhan
{
    public static class JiXianTiaoZhanActivity
    {
        public static async Task<JsonResult> GetActivityInfo(JiXianTiaoZhanController mJiXianTiaoZhanController)
        {
            JiXianTiaoZhan_GetActivityInfo_Result mResult = new JiXianTiaoZhan_GetActivityInfo_Result();
            try
            {
                mResult.nErrorCode = NetErrorCode.Success;
                mResult.ActivityInfo = JiXianTiaoZhanModel.Instance.mExcelDataCollection;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error("GetActivityInfo Exception: " + ex.Message + " | " + ex.StackTrace);
                mResult.nErrorCode = NetErrorCode.ServerInnerEexception;
            }

            return new JsonResult(mResult);
        }

        public static async Task<JsonResult> UploadChengji(JiXianTiaoZhanController mJiXianTiaoZhanController)
        {
            JiXianTiaoZhan_UploadChengji_Result mResult = new JiXianTiaoZhan_UploadChengji_Result();
            try
            {
                IFormCollection collection = await mJiXianTiaoZhanController.Request.ReadFormAsync();
                if (!collection.ContainsKey("userId") || !collection.ContainsKey("nActivityId") || !collection.ContainsKey("nTime") || !collection.ContainsKey("nScore"))
                {
                    mResult.nErrorCode = NetErrorCode.ParamError;
                    return new JsonResult(mResult);
                }

                string userId = collection["userId"];
                uint nActivityId = uint.Parse(collection["nActivityId"]);
                uint nTime = uint.Parse(collection["nTime"]);
                uint nScore = uint.Parse(collection["nScore"]);

                if (string.IsNullOrWhiteSpace(userId) || userId.Length < 6 || nActivityId <= 0)
                {
                    mResult.nErrorCode = NetErrorCode.ParamError;
                    return new JsonResult(mResult);
                }

                bool BOpSuccess = await CreateOrUpdateDb(mJiXianTiaoZhanController, userId, nActivityId, nTime, nScore);
                if (!BOpSuccess)
                {
                    mResult.nErrorCode = NetErrorCode.DbOpError;
                    return new JsonResult(mResult);
                }

                mResult.nErrorCode = NetErrorCode.Success;
                return new JsonResult(mResult);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error("UploadChengji Exception: " + ex.Message + " | " + ex.StackTrace);
                mResult.nErrorCode = NetErrorCode.ServerInnerEexception;
                return new JsonResult(mResult);
            }
        }

        public static async Task<JsonResult> GetRankList(JiXianTiaoZhanController mJiXianTiaoZhanController)
        {
            JiXianTiaoZhan_GetRankListInfo_Result mResult = new JiXianTiaoZhan_GetRankListInfo_Result();
            try
            {
                if (mJiXianTiaoZhanController.nActivityId <= 0)
                {
                    mResult.nErrorCode = NetErrorCode.ActivityNoOpen;
                    return new JsonResult(mResult);
                }

                IFormCollection collection = await mJiXianTiaoZhanController.Request.ReadFormAsync();
                if (!collection.ContainsKey("userId"))
                {
                    mResult.nErrorCode = NetErrorCode.ParamError;
                    return new JsonResult(mResult);
                }

                string userId = collection["userId"];
                if (string.IsNullOrWhiteSpace(userId) || userId.Length < 6)
                {
                    mResult.nErrorCode = NetErrorCode.ParamError;
                    return new JsonResult(mResult);
                }

                WorldDBContext mDbContex = mJiXianTiaoZhanController.worldDBContext;
                int nActivityId = mJiXianTiaoZhanController.nActivityId;

                List<JiXianTiaoZhanJoinDbUser> mSameActivityList = new List<JiXianTiaoZhanJoinDbUser>();
                await foreach (var v in JiXianTiaoZhanDbCompileQuery.GetActivityUserList(mDbContex, nActivityId))
                {
                    mSameActivityList.Add(v);
                }

                var mSortQueue = mSameActivityList.OrderByDescending((x) => x.mJiXianTiaoZhanInfo.nTime)
                    .ThenByDescending((x) => x.mJiXianTiaoZhanInfo.nScore)
                    .ThenBy((x) => x.mJiXianTiaoZhanInfo.nUserId).Take(50);

                List<ResultPlayerRankInfo> mPlayerRankInfoList = new List<ResultPlayerRankInfo>();
                uint nRankIndex = 1;
                foreach (var v in mSortQueue)
                {
                    ResultPlayerRankInfo mInfo = GetResultPlayerInfo(v.mJiXianTiaoZhanInfo);
                    mInfo.nRank = nRankIndex++;
                    mInfo.strName = v.mDbUser.nick;
                    mInfo.iconUrl = v.mDbUser.icon;
                    mPlayerRankInfoList.Add(mInfo);
                }

                JiXianTiaoZhanJoinDbUser mDbCombineInfo = mSameActivityList.FirstOrDefault((x) => x.mJiXianTiaoZhanInfo.nUserId == userId);
                if (mDbCombineInfo != null)
                {
                    int nCount = mSameActivityList.Where((x) =>
                          (x.mJiXianTiaoZhanInfo.nTime > mDbCombineInfo.mJiXianTiaoZhanInfo.nTime ||
                          (x.mJiXianTiaoZhanInfo.nTime == mDbCombineInfo.mJiXianTiaoZhanInfo.nTime && x.mJiXianTiaoZhanInfo.nScore > mDbCombineInfo.mJiXianTiaoZhanInfo.nScore) ||
                          (x.mJiXianTiaoZhanInfo.nTime == mDbCombineInfo.mJiXianTiaoZhanInfo.nTime && x.mJiXianTiaoZhanInfo.nScore == mDbCombineInfo.mJiXianTiaoZhanInfo.nScore &&
                            x.mJiXianTiaoZhanInfo.nUserId.CompareTo(mDbCombineInfo.mJiXianTiaoZhanInfo.nUserId) < 0))).Count();
                    mDbCombineInfo.mJiXianTiaoZhanInfo.nRank = (uint)nCount + 1;
                }

                mResult.nErrorCode = NetErrorCode.Success;
                mResult.mPlayerRankInfoList = mPlayerRankInfoList;
                if (mDbCombineInfo != null)
                {
                    var mRankInfo = GetResultPlayerInfo(mDbCombineInfo.mJiXianTiaoZhanInfo);
                    mRankInfo.strName = mDbCombineInfo.mDbUser.nick;
                    mRankInfo.iconUrl = mDbCombineInfo.mDbUser.icon;
                    mResult.MyRankInfo = mRankInfo;
                }
                return new JsonResult(mResult);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error("GetRankList Exception: " + ex.Message + " | " + ex.StackTrace);
                mResult.nErrorCode = NetErrorCode.ServerInnerEexception;
                return new JsonResult(mResult);
            }
        }

        private static async Task<bool> CreateOrUpdateDb(JiXianTiaoZhanController mJiXianTiaoZhanController, string userId, uint nActivityId, uint nTime, uint nScore)
        {
            WorldDBContext mDbContex = mJiXianTiaoZhanController.worldDBContext;
            DbSet<DbJiXianTiaoZhanInfo> mDbSet = mDbContex.JiXianTiaoZhanInfoList;

            try
            {
                DbJiXianTiaoZhanInfo mDbInfo = await mDbSet.FindAsync(userId);
                bool bCreate = false;
                if (mDbInfo == null)
                {
                    mDbInfo = new DbJiXianTiaoZhanInfo();
                    bCreate = true;
                }

                if (nActivityId == mDbInfo.nActivityId)
                {
                    uint nOldTime = mDbInfo.nTime;
                    uint nOldScore = mDbInfo.nScore;

                    bool bIsBest = false;

                    if (nTime > nOldTime)
                    {
                        bIsBest = true;
                    }
                    else if (nTime == nOldTime && nScore > nOldScore)
                    {
                        bIsBest = true;
                    }

                    if (!bIsBest)
                    {
                        return true;
                    }
                }

                mDbInfo.nUserId = userId;
                mDbInfo.nActivityId = nActivityId;
                mDbInfo.nTime = nTime;
                mDbInfo.nScore = nScore;

                if (bCreate)
                {
                    await mDbSet.AddAsync(mDbInfo);
                }
                else
                {
                    mDbSet.Update(mDbInfo);
                }

                await mDbContex.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException e)
            {
                NLog.LogManager.GetCurrentClassLogger().Error("CreateOrUpdateDb DbUpdateConcurrencyException: " + e.Message + " | " + e.StackTrace);
            }
            catch (DbUpdateException e)
            {
                NLog.LogManager.GetCurrentClassLogger().Error("CreateOrUpdateDb DbUpdateException: " + e.Message + " | " + e.StackTrace);
            }
            catch (OperationCanceledException e)
            {
                NLog.LogManager.GetCurrentClassLogger().Error("CreateOrUpdateDb OperationCanceledException: " + e.Message + " | " + e.StackTrace);
            }
            catch (Exception e)
            {
                NLog.LogManager.GetCurrentClassLogger().Error("CreateOrUpdateDb Exception: " + e.Message + " | " + e.StackTrace);
            }

            return false;
        }

        public static ResultPlayerRankInfo GetResultPlayerInfo(DbJiXianTiaoZhanInfo mDbInfo)
        {
            ResultPlayerRankInfo mInfo = new ResultPlayerRankInfo();
            mInfo.nUserId = mDbInfo.nUserId;
            mInfo.nTime = mDbInfo.nTime;
            mInfo.nScore = mDbInfo.nScore;
            mInfo.nRank = mDbInfo.nRank;
            mInfo.iconUrl = String.Empty;
            mInfo.strName = String.Empty;
            return mInfo;
        }

    }
}
