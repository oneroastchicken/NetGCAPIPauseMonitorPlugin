
using Volo.Abp;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Modularity;

namespace APT.GCPauseMonitorPlugin
{
    [DependsOn(typeof(AbpBackgroundWorkersModule))]
    public class GCPauseMonitorPluginModule : AbpModule
    {
        //TODO 生产环境异常
        //public override void PreConfigureServices(ServiceConfigurationContext context)
        //{
        //    Configure<AbpDbContextOptions>(options =>
        //    {
        //        options.PreConfigure(configurationContext =>
        //        {
        //            configurationContext.DbContextOptions.AddInterceptors(new SqlExecutionInterceptor());
        //        });
        //    });
        //}

        public override async void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            await context.AddBackgroundWorkerAsync<GCMonitoringWorker>();
        }


    }

    //TODO 生产环境异常
    //public static class GCPauseMonitorOptions
    //{
    //    public static AbpDbContextOptions AddPGSqlInterceptor(this AbpDbContextOptions options)
    //    {
    //        options.PreConfigure(configurationContext =>
    //        {
    //            configurationContext.DbContextOptions.AddInterceptors(new SqlExecutionInterceptor());
    //        });
    //        return options;
    //    }
    //}

}