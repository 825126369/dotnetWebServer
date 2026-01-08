
using DTBWebServer.ExcelCS;
using System.Diagnostics;

namespace DTBWebServer.GameLogic.JiXianTiaoZhan
{

    public class JiXianTiaoZhanModel : Singleton<JiXianTiaoZhanModel>
    {
        public ExcelDataCollection<JiXianChallengeData> mExcelDataCollection;
        public void Init()
        {
            string content = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "ExcelData/JiXianChallengeData.json"));
            mExcelDataCollection = Newtonsoft.Json.JsonConvert.DeserializeObject<ExcelDataCollection<JiXianChallengeData>>(content);
            Debug.Assert(mExcelDataCollection != null, "mExcelDataCollection == nul");
        }

        public bool CheckJsonConfigFileError(string filePath = null)
        {
            try
            {
                if (filePath == null)
                {
                    filePath = Path.Combine(AppContext.BaseDirectory, "ExcelData/JiXianChallengeData.json");
                }

                if (!File.Exists(filePath))
                {
                    return false;
                }

                string content = File.ReadAllText(filePath);
                var mExcelDataCollection = Newtonsoft.Json.JsonConvert.DeserializeObject<ExcelDataCollection<JiXianChallengeData>>(content);
                if (mExcelDataCollection == null)
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(e.Message + " | " + e.StackTrace);
                return false;
            }

            return true;
        }

        public int GetCurrentActivityId()
        {
            try
            {
                DateTime now = DateTime.Now;//北京时间
                foreach (var v in mExcelDataCollection.mActivityDataList)
                {
                    DateTime beginTime = TimeUtility.GetLocalTimeFromDateString(v.activityBeginTime);
                    DateTime endTime = TimeUtility.GetLocalTimeFromDateString(v.activityEndTime);

                    if (now >= beginTime && now < endTime)
                    {
                        return v.challengeId;
                    }
                }
            }
            catch (Exception e)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(e.Message + " | " + e.StackTrace);
            }

            return -1;
        }


    }

}
