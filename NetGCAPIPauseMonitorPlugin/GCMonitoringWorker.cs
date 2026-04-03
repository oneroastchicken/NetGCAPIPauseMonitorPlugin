using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Threading;
namespace APT.GCPauseMonitorPlugin
{
    public class GCMonitoringWorker : AsyncPeriodicBackgroundWorkerBase
    {
        private readonly IGCPauseMonitorService _monitorService;

        public GCMonitoringWorker(
            AbpAsyncTimer timer,
            IServiceScopeFactory serviceScopeFactory,
            IGCPauseMonitorService monitorService)
            : base(timer, serviceScopeFactory)
        {
            _monitorService = monitorService;
            Timer.Period = 1000;
            Timer.RunOnStart = true;
        }

        protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
        {
            var stats = _monitorService.GetStatistics();

            //Logger.LogInformation(
            //    $"GC统计{stats.PodKey}: " +
            //    $"总暂停={stats.TotalPauses}, " +
            //    $"总耗时={stats.TotalPauseTimeMs}ms, " +
            //    $"上次暂停={stats.LastPauseMs}ms, " +
            //    $"最长暂停={stats.MaxPauseMs}ms," +
            //    $"平均暂停={stats.AvgPauseMs}ms");


            FixedSizeCollections.Add(typeof(GCPause).Name, stats, 50);

            await Task.CompletedTask;
        }
    }
}