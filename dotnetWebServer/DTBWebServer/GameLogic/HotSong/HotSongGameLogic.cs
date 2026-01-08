using DTBWebServer.Controllers.HotSong;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace DTBWebServer.GameLogic.HotSong
{
    public static class HotSongGameLogic
    {
        public static async Task<JsonResult> GetRankList(HotSongController mHotSongController, int nRankCount)
        {
            HotSong_GetRankListInfo_Result mResult = new HotSong_GetRankListInfo_Result();
            try
            {
                int nWeek = ISOWeek.GetWeekOfYear(DateTime.Now) - 1;
                var mRankList = mHotSongController.worldDBContext.t_hot.Where((x) => x.week_num == nWeek).OrderByDescending((x) => x.num).Take(nRankCount).AsAsyncEnumerable();

                List<ResultHotSongRankInfo> mRankInfoList = new List<ResultHotSongRankInfo>();
                await foreach (var v in mRankList)
                {
                    ResultHotSongRankInfo mRankInfo = new ResultHotSongRankInfo();
                    mRankInfo.music_id = v.music_id.Value;
                    mRankInfo.num = v.num.Value;
                    mRankInfoList.Add(mRankInfo);
                }
                mResult.mHotSongRankInfoList = mRankInfoList;

                mResult.nErrorCode = NetErrorCode.Success;
                return new JsonResult(mResult);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error("GetRankList Exception: " + ex.Message + " | " + ex.StackTrace);
                mResult.nErrorCode = NetErrorCode.ServerInnerEexception;
                return new JsonResult(mResult);
            }
        }

    }
}
