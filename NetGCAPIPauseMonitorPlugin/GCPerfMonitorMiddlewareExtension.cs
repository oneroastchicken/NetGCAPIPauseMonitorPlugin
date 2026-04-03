using Microsoft.AspNetCore.Builder;

namespace APT.GCPauseMonitorPlugin
{
    public static class GCPerfMonitorMiddlewareExtension
    {
        public static IApplicationBuilder UseGCPerfMonitor(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GCPerfMonitorMiddleware>();
        }
    }
}
