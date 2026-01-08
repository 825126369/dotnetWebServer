using DTBWebServer.ExcelCS;

namespace DTBWebServer.GameLogic.JiXianTiaoZhan
{
    public class ResultPlayerRankInfo
    {
        public string nUserId { get; set; }
        public uint nTime { get; set; }
        public uint nScore { get; set; }
        public uint nRank { get; set; }
        public string iconUrl { get; set; }
        public string strName { get; set; }
    }

    public class JiXianTiaoZhan_GetActivityInfo_Result:ResultBase
    {
        public ExcelDataCollection<JiXianChallengeData> ActivityInfo { get; set; }
    }

    public class JiXianTiaoZhan_UploadChengji_Result : ResultBase
    {
        
    }

    public class JiXianTiaoZhan_GetRankListInfo_Result : ResultBase
    {
        public List<ResultPlayerRankInfo> mPlayerRankInfoList { get; set; }
        public ResultPlayerRankInfo MyRankInfo { get; set; }
    }
}
