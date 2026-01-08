namespace DTBWebServer.ExcelCS
{
    public class JiXianChallengeData
    {

        /// <summary>
        /// 序号id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 挑战id
        /// </summary>
        public int challengeId { get; set; }

        /// <summary>
        /// 挑战名称
        /// </summary>
        public string challengeName { get; set; }

        /// <summary>
        /// 歌曲id
        /// </summary>
        public string[] songId { get; set; }

        /// <summary>
        /// 歌曲难度
        /// </summary>
        public string[] songDifficult { get; set; }

        /// <summary>
        /// 封面名称
        /// </summary>
        public string coverName { get; set; }

        /// <summary>
        /// 场景名称
        /// </summary>
        public string sceneName { get; set; }

        /// <summary>
        /// 血量值
        /// </summary>
        public int HP { get; set; }

        /// <summary>
        /// 扣血系数
        /// </summary>
        public float[] HPCoefficient2 { get; set; }

        /// <summary>
        /// 挑战规则id
        /// </summary>
        public int challengeRulesId { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public string activityBeginTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public string activityEndTime { get; set; }
    }

}
