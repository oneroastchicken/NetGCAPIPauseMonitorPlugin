using System.Diagnostics;

namespace APT.GCPauseMonitorPlugin
{
    public static class GCPerfExecutionContext
    {
        private static readonly AsyncLocal<List<SqlTimingDetail>> _sqlTimingDetails = new AsyncLocal<List<SqlTimingDetail>>();
        private static readonly AsyncLocal<Stopwatch> _totalOpergTimer = new AsyncLocal<Stopwatch>();
        private static readonly AsyncLocal<bool> _isMonitoring = new AsyncLocal<bool>();

        public static bool IsMonitoring => _isMonitoring.Value;

        public static void StartMonitoring()
        {
            _totalOpergTimer.Value = Stopwatch.StartNew();
            _sqlTimingDetails.Value = new List<SqlTimingDetail>();
            _isMonitoring.Value = true;
        }

        public static void AddSqlTiming(SqlTimingDetail timing)
        {
            if (!_isMonitoring.Value) return;

            var list = GetTimingDetails();
            list.Add(timing);
        }

        public static TimingResult EndMonitoring()
        {
            if (_totalOpergTimer?.Value == null || !_isMonitoring.Value)
                return null;

            _totalOpergTimer.Value.Stop();
            var totalTime = _totalOpergTimer.Value.ElapsedMilliseconds;

            var details = GetTimingDetails();

            var sqlTotalTime = details.Sum(t => t.SqlExecutionTime);

            var queryCount = details.Count(t => t.CommandType == "Query");
            var nonQueryCount = details.Count(t => t.CommandType == "NonQuery");
            var scalarCount = details.Count(t => t.CommandType == "Scalar");


            var result = new TimingResult
            {
                TotalTime = totalTime,
                SqlTotalTime = sqlTotalTime,
                QueryCount = queryCount,
                NonQueryCount = nonQueryCount,
                ScalarCount = scalarCount
            };


            return result;
        }

        private static List<SqlTimingDetail> GetTimingDetails()
        {
            return _sqlTimingDetails.Value ?? new List<SqlTimingDetail>();
        }


        public static void Clear()
        {
            _sqlTimingDetails.Value = new List<SqlTimingDetail>();
            _totalOpergTimer.Value = null;
            _isMonitoring.Value = false;
        }
    }

    public class SqlTimingDetail
    {
        public string CommandType { get; set; } 
        public long SqlExecutionTime { get; set; } 
    }

    public class TimingResult
    {
        public long TotalTime { get; set; }
        public long SqlTotalTime { get; set; }
        public int QueryCount { get; set; }
        public int NonQueryCount { get; set; }
        public int ScalarCount { get; set; }
    }
}