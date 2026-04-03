using Serilog;
using System.Collections.Concurrent;
using System.Diagnostics.Tracing;

namespace APT.GCPauseMonitorPlugin
{

    public sealed class GCPauseEventListener : EventListener
    {
        public event Action<double> GCPauseOccurred;
        private readonly ConcurrentDictionary<string, bool> _processedSources = new();

        public void Initialize()
        {
            foreach (var eventSource in EventSource.GetSources())
            {
                ProcessEventSource(eventSource);
            }
        }

        protected override void OnEventSourceCreated(EventSource eventSource)
        {
            base.OnEventSourceCreated(eventSource);
            ProcessEventSource(eventSource);
        }

        private void ProcessEventSource(EventSource eventSource)
        {
            if (_processedSources.TryAdd(eventSource.Name, true))
            {
                if (eventSource.Name == "Microsoft-Windows-DotNETRuntime" ||
                    eventSource.Name == "System.Runtime")
                {
                    try
                    {
                        EnableEvents(eventSource, EventLevel.Informational, (EventKeywords)0x1);
                    }
                    catch (System.Exception ex)
                    {
                        Log.Error($"Failed to enable event source: {eventSource.Name}, ERR: {ex.Message}");
                    }
                }
            }
        }

        protected override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            if (eventData.EventId == 202 && eventData.Payload?.Count >= 1)
            {
                try
                {
                    // Log.Information($"检测GC 暂停时间: PauseTime:{string.Join("|", eventData.Payload)}");

                    // Payload: 0|0|3|32425
                    // Payload[0]: 可能代表 GC 发生的代（Generation）。通常 0 表示 Gen0 GC，1 表示 Gen1 GC，2 表示 Gen2 GC
                    // Payload[1]: 可能代表 GC 类型或状态，如是否是 Full GC 或其他类型的 GC
                    // Payload[2]: 这个字段可以表示 GC 触发的原因或其他与 GC 相关的信息，如内存压力等
                    // Payload[3]: GC 暂停时间，单位是微秒（µs）

                  
                    //Log.Information($"GC eventData.Payload: {string.Join("|", eventData.Payload)}");
                    long microseconds = Convert.ToInt64(eventData.Payload[3]);
                    double milliseconds = microseconds / 1000.0;
                    GCPauseOccurred?.Invoke(milliseconds);
                }
                catch (System.Exception ex)
                {
                    Log.Error($"Failed to process GC event: {ex.Message}");
                }
            }
        }
    }
}
