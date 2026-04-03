namespace APT.GCPauseMonitorPlugin
{
    public class GCPause
    {
        /// <summary>
        /// 启动时间
        /// </summary>
        public string StartUpTime { get; set; }

        /// <summary>
        /// GC统计
        /// </summary>
        public GCPauseStats Statistics { get; set; }
    }

    public class GCPauseStats
    {
        /// <summary>
        /// 时间
        /// </summary>
        public string Time { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        /// <summary>
        /// 最后暂停时间（ms）
        /// </summary>
        public double LastPause { get; set; }

        /// <summary>
        /// 最大暂停时间（ms）
        /// </summary>
        public double MaxPause { get; set; }

        /// <summary>
        /// 平均暂停时间（ms）
        /// </summary>
        public double AvgPause => TotalCount == 0 ? 0 : TotalPause / TotalCount;

        /// <summary>
        /// 总暂停时间（ms）
        /// </summary>
        public double TotalPause { get; set; }

        /// <summary>
        /// 总暂停次数
        /// </summary>
        public long TotalCount { get; set; }
    }
}
