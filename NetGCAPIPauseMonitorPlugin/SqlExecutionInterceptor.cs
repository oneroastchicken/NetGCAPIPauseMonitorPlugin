using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Collections.Concurrent;
using System.Data.Common;
using System.Diagnostics;

namespace APT.GCPauseMonitorPlugin
{
    public class SqlExecutionInterceptor : DbCommandInterceptor
    {
        private readonly ConcurrentDictionary<DbCommand, Stopwatch> _stopwatches = new();

        // ===== 通用开始方法 =====
        private void StartTiming(DbCommand command)
        {
            if (GCMonitoringPod.MonitSql)
                _stopwatches[command] = Stopwatch.StartNew();
        }

        // ===== 通用结束方法 =====
        private void TrackExecution(DbCommand command, CommandExecutedEventData eventData)
        {
            if (!GCMonitoringPod.MonitSql) return;

            if (_stopwatches.TryRemove(command, out var sw))
            {
                sw.Stop();
                var sqlTime = eventData?.Duration.TotalMilliseconds ?? sw.ElapsedMilliseconds;
                var commandType = ClassifyCommand(command.CommandText);

                // 直接写入 AsyncLocal List
                GCPerfExecutionContext.AddSqlTiming(new SqlTimingDetail
                {
                    CommandType = commandType,
                    SqlExecutionTime = (long)sqlTime
                });
            }
        }

        // ===== 异步 =====
        public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
            DbCommand command, CommandEventData eventData,
            InterceptionResult<DbDataReader> result, CancellationToken cancellationToken = default)
        {
            StartTiming(command);
            return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
        }

        public override ValueTask<DbDataReader> ReaderExecutedAsync(
            DbCommand command, CommandExecutedEventData eventData,
            DbDataReader result, CancellationToken cancellationToken = default)
        {
            TrackExecution(command, eventData);
            return base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
        }

        public override ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(
            DbCommand command, CommandEventData eventData,
            InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            StartTiming(command);
            return base.NonQueryExecutingAsync(command, eventData, result, cancellationToken);
        }

        public override ValueTask<int> NonQueryExecutedAsync(
            DbCommand command, CommandExecutedEventData eventData,
            int result, CancellationToken cancellationToken = default)
        {
            TrackExecution(command, eventData);
            return base.NonQueryExecutedAsync(command, eventData, result, cancellationToken);
        }

        // ===== 同步 =====
        public override InterceptionResult<DbDataReader> ReaderExecuting(
            DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
        {
            StartTiming(command);
            return base.ReaderExecuting(command, eventData, result);
        }

        public override DbDataReader ReaderExecuted(
            DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
        {
            TrackExecution(command, eventData);
            return base.ReaderExecuted(command, eventData, result);
        }

        public override InterceptionResult<int> NonQueryExecuting(
            DbCommand command, CommandEventData eventData, InterceptionResult<int> result)
        {
            StartTiming(command);
            return base.NonQueryExecuting(command, eventData, result);
        }

        public override int NonQueryExecuted(
            DbCommand command, CommandExecutedEventData eventData, int result)
        {
            TrackExecution(command, eventData);
            return base.NonQueryExecuted(command, eventData, result);
        }

        // ===== SQL 分类 =====
        private string ClassifyCommand(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql)) return "Query";
            var normalized = sql.Trim().ToUpperInvariant();

            if (normalized.StartsWith("SELECT COUNT(") ||
                normalized.StartsWith("SELECT AVG(") ||
                normalized.StartsWith("SELECT SUM(") ||
                normalized.StartsWith("SELECT MAX(") ||
                normalized.StartsWith("SELECT MIN(") ||
                normalized.StartsWith("SELECT EXISTS("))
                return "Scalar";

            if (normalized.StartsWith("SELECT")) return "Query";

            if (normalized.StartsWith("INSERT ") || normalized.StartsWith("UPDATE ") || normalized.StartsWith("DELETE "))
                return "NonQuery";

            return "Query";
        }
    }
}
