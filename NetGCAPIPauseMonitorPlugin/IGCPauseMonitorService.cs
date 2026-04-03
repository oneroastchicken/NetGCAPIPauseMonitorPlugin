using Volo.Abp.DependencyInjection;

namespace APT.GCPauseMonitorPlugin
{
    public interface IGCPauseMonitorService : ITransientDependency
    {
        public GCPause GetStatistics();
    }
}
