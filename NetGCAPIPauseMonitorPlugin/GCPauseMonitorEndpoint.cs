using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace APT.GCPauseMonitorPlugin
{
    public static class GCPauseMonitorEndpoint
    {
        public static IEndpointRouteBuilder MapGcMonitor(
            this IEndpointRouteBuilder endpoints, string system)
        {

           
            endpoints.MapGet($"/api/{system}/gc-monitor", () =>
            {
                var pause = FixedSizeCollections.GetAll<GCPause>(typeof(GCPause).Name);
                var result = new Dictionary<string, object>
                {
                    [string.Concat(system, "-", GCMonitoringPod.Key.ToString())] =
                    new
                    {
                        Pauses = pause.Select(s => s.Statistics).OrderByDescending(s => s.TotalCount).ToList(),
                        StartUpTime = pause.FirstOrDefault()?.StartUpTime
                    }
                };
                return Results.Json(result);
            });

            endpoints.MapGet($"/api/{system}/pod-key", () =>
            {
                var pause = FixedSizeCollections.GetAll<GCPause>(typeof(GCPause).Name);

                return Results.Ok(string.Concat(system, "-", GCMonitoringPod.Key.ToString()));
            });

            endpoints.MapGet($"/api/{system}/gc-perf", (string? path, string? sort) =>
            {
                var perfs = FixedSizeCollections.GetAll<GCPerf>(typeof(GCPerf).Name)
                                                .WhereIf(!string.IsNullOrWhiteSpace(path), s => s.Path == path);

                if (!string.IsNullOrEmpty(sort))
                {
                    if (sort.Equals("time"))
                    {
                        perfs = perfs.OrderByDescending(s => s.BeginTime);
                    }
                    if (sort.Equals("elapsed"))
                    {
                        perfs = perfs.OrderByDescending(s => s.Elapsed);
                    }
                }

                var result = new Dictionary<string, object>
                {
                    [string.Concat(system, "-", GCMonitoringPod.Key.ToString())] =
                    new
                    {
                        Perfs = perfs.ToList()
                    }
                };
                return Results.Json(result);
            });


            endpoints.MapGet($"/api/{system}/gc-perf-set", (string? podKey, string? path, long? time,bool? clear, bool? monitSql) =>
            {

                if (clear.HasValue && clear.Value) FixedSizeCollections.Clear<GCPerf>(typeof(GCPerf).Name);
                if (monitSql.HasValue) GCMonitoringPod.MonitSql = monitSql.Value;
                if (!string.IsNullOrEmpty(path) && GCMonitoringPod.Key.Equals(podKey))
                {
                    GCMonitoringPod.PerfPath = path;
                    GCMonitoringPod.LimitTime = time ?? 100;
                }
                return Results.Ok(true);

            });

            return endpoints;
        }
    }
}