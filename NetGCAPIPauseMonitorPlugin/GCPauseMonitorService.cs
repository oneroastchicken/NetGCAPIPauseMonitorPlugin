using Microsoft.Extensions.Logging;

namespace APT.GCPauseMonitorPlugin
{
    public class GCPauseMonitorService : IGCPauseMonitorService, IDisposable
    {
        private readonly ILogger<GCPauseMonitorService> _logger;
        private readonly GCPauseEventListener _listener;
        private readonly object _lockObj = new object();

        private long _totalPauses;
        private double _totalPauseTimeMs;  
        private double _lastPauseMs;       
        private double _maxPauseMs;
        private DateTime _startTime = DateTime.Now;
        private DateTime _checkTime = DateTime.Now.AddMinutes(1);

        public GCPauseMonitorService(ILogger<GCPauseMonitorService> logger)
        {
            _logger = logger;
            _listener = new GCPauseEventListener();

            _listener.Initialize();
            _listener.GCPauseOccurred += OnGCPause;
        }

        private void OnGCPause(double pauseTimeMs)
        {

            var currentTime = DateTime.Now;

            if (currentTime > _checkTime)
            {
                Interlocked.Increment(ref _totalPauses);

                lock (_lockObj)
                {
                    if (pauseTimeMs > 0)
                    {
                        _lastPauseMs = pauseTimeMs;
                    }

                    // 更新最大值
                    if (pauseTimeMs > _maxPauseMs)
                    {
                        _maxPauseMs = pauseTimeMs;
                    }

                    // 累加总时间
                    _totalPauseTimeMs += pauseTimeMs;
                }

                // 记录长时间暂停警告
                //if (pauseTimeMs > 100)
                //{
                //    _logger.LogWarning("检测到长时间 GC 暂停: {PauseTime:F3}ms", pauseTimeMs);
                //}
            }
        }

        public GCPause GetStatistics()
        {
            lock (_lockObj)
            {
                return new GCPause
                {
                    StartUpTime = _startTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    Statistics = new GCPauseStats
                    {
                        TotalCount = _totalPauses,
                        TotalPause = _totalPauseTimeMs,
                        LastPause = _lastPauseMs,
                        MaxPause = _maxPauseMs
                    }
                };
            }
        }

        public void Dispose()
        {
            _listener?.Dispose();
            _logger.LogInformation("GC 暂停时间监控已停止");
        }
    }
}