using DTBWebServer.ExcelCS;

namespace DTBWebServer.GameLogic.HotSong
{
    public class ResultHotSongRankInfo
    {
        public int music_id { get; set; }
        public int num { get; set; }
    }

    public class HotSong_GetRankListInfo_Result : ResultBase
    {
        public List<ResultHotSongRankInfo> mHotSongRankInfoList { get; set; }
    }

}
