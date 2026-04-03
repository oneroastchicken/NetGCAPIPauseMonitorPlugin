namespace APT.GCPauseMonitorPlugin
{
    public class GCPerf
    {

        /// <summary>
        /// 响应KEY
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// URL地址
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime BeginTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime => BeginTime.AddMilliseconds(Elapsed);

        /// <summary>
        /// 耗时（毫秒）
        /// </summary>
        public long Elapsed { get; set; }

        /// <summary>
        /// SQL耗时（毫秒）
        /// </summary>
        public long SqlElapsed { get; set; }

        /// <summary>
        /// SQL查询次数
        /// </summary>
        public string SqlCount { get; set; }

        /// <summary>
        /// 可用工作线程数(Worker)
        /// </summary>
        public int WorkerBefore { get; set; }

        /// <summary>
        /// 可用工作线程数(Worker)
        /// </summary>
        public int WorkerAfter { get; set; }


        /// <summary>
        /// 可用I/O 线程数 (IOCP)
        /// </summary>
        public int IOBefore { get; set; }

        /// <summary>
        /// 可用I/O 线程数 (IOCP)
        /// </summary>
        public int IOAfter { get; set; }


        public int GC0After { get; set; }
        public int GC1After { get; set; }
        public int GC2After { get; set; }

        public int GC0Before { get; set; }
        public int GC1Before { get; set; }
        public int GC2Before { get; set; }


        /// <summary>
        /// 初代
        /// </summary>
        public int Gen0 => GC0After - GC0Before;


        /// <summary>
        /// 一代
        /// </summary>
        public int Gen1 => GC1After - GC1Before;


        /// <summary>
        /// 二代
        /// </summary>
        public int Gen2 => GC1After - GC1Before;
    }

}
