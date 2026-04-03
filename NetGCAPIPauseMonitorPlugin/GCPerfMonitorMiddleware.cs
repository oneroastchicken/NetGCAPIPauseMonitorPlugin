using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace APT.GCPauseMonitorPlugin
{
    public class GCPerfMonitorMiddleware
    {
        private readonly RequestDelegate _next;

        public GCPerfMonitorMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            if (GCMonitoringPod.PerfPath == null || !GCMonitoringPod.PerfPath.StartsWith("/") || !context.Request.Path.Equals(GCMonitoringPod.PerfPath))
            {
                await _next(context);
                return;
            }

            var sw = Stopwatch.StartNew();
            var beginTime = DateTime.Now;

            //GCPerfExecutionContext.StartMonitoring();

            var gc0Before = GC.CollectionCount(0);
            var gc1Before = GC.CollectionCount(1);
            var gc2Before = GC.CollectionCount(2);

            ThreadPool.GetAvailableThreads(out var workerBefore, out var ioBefore);

            context.Response.OnStarting(state =>
            {
                try
                {
                    var httpCtx = (HttpContext)state;

                    sw.Stop();
                    var elapsed = sw.ElapsedMilliseconds;

                    //var timing = GCPerfExecutionContext.EndMonitoring();
                    //var elapsed = timing.TotalTime;

                    if (elapsed > GCMonitoringPod.LimitTime)
                    {
                        var gc0After = GC.CollectionCount(0);
                        var gc1After = GC.CollectionCount(1);
                        var gc2After = GC.CollectionCount(2);
                        ThreadPool.GetAvailableThreads(out var workerAfter, out var ioAfter);

                        var key = Guid.NewGuid().ToString("N");

                        //string sqlcount = $"{timing.QueryCount}-{timing.NonQueryCount}-{timing.ScalarCount}";


                        string sqlcount = "0-0-0";
                        httpCtx.Response.Headers["x-gc-sqlcount"] = sqlcount;
                        httpCtx.Response.Headers["x-gc-sqlelapsed"] = "0"; //timing.SqlTotalTime.ToString();

                        httpCtx.Response.Headers["x-gc-key"] = key;
                        httpCtx.Response.Headers["x-gc-elapsed"] = elapsed.ToString();
                        httpCtx.Response.Headers["x-gc-worker"] = $"{workerBefore}-{workerAfter}";
                        httpCtx.Response.Headers["x-gc-io"] = $"{ioBefore}-{ioAfter}";
                        httpCtx.Response.Headers["x-gc-gen"] = $"{gc0After - gc0Before}-{gc1After - gc1Before}-{gc2After - gc2Before}";

                        var perf = new GCPerf
                        {
                            Key = key,
                            Path = httpCtx.Request.Path,
                            Elapsed = elapsed,
                            SqlElapsed = 0,//timing.SqlTotalTime,
                            SqlCount = sqlcount,
                            WorkerBefore = workerBefore,
                            IOBefore = ioBefore,
                            WorkerAfter = workerAfter,
                            IOAfter = ioAfter,
                            GC0Before = gc0Before,
                            GC1Before = gc1Before,
                            GC2Before = gc2Before,
                            GC0After = gc0After,
                            GC1After = gc1After,
                            GC2After = gc2After,
                            BeginTime = beginTime
                        };
                        FixedSizeCollections.Add(typeof(GCPerf).Name, perf, 1000);
                    }

                }
                catch { }

                return Task.CompletedTask;

            }, context);

            await _next(context);
        }
    }
}