using System.Net;
using System.Net.Sockets;

namespace APT.GCPauseMonitorPlugin
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    public static class FixedSizeCollections
    {
        private static readonly ConcurrentDictionary<string, object> _collections = new();

        public static void Add<T>(string key, T item, int maxSize = 60)
        {
            var collection = GetOrCreateCollection<T>(key, maxSize);
            collection.Add(item);
        }

        public static List<T> GetAll<T>(string key)
        {
            var collection = GetOrCreateCollection<T>(key);
            return collection.GetAll();
        }

        public static void Clear<T>(string key)
        {
            var collection = GetOrCreateCollection<T>(key);
            collection.Clear();
        }

        public static int Count<T>(string key)
        {
            var collection = GetOrCreateCollection<T>(key);
            return collection.Count;
        }

        private static FixedSizeCollection<T> GetOrCreateCollection<T>(string key, int maxSize = 60)
        {
            return (FixedSizeCollection<T>)_collections.GetOrAdd(key, _ => new FixedSizeCollection<T>(maxSize));
        }

        private class FixedSizeCollection<T>
        {
            private readonly ConcurrentQueue<T> _queue = new();
            private readonly object _lock = new object();
            public int MaxSize { get; private set; }

            public FixedSizeCollection(int maxSize)
            {
                MaxSize = maxSize;
            }

            public void Add(T item)
            {
                lock (_lock)
                {
                    _queue.Enqueue(item);
                    while (_queue.Count > MaxSize)
                    {
                        _queue.TryDequeue(out _);
                    }
                }
            }

            public List<T> GetAll()
            {
                lock (_lock)
                {
                    return _queue.ToList();
                }
            }

            public void Clear()
            {
                lock (_lock)
                {
                    while (_queue.Count > 0)
                    {
                        _queue.TryDequeue(out _);
                    }
                }
            }

            public int Count => _queue.Count;
        }
    }

    public static class GCMonitoringPod
    {
        public static readonly string Key = IP ?? Dns.GetHostAddresses(Dns.GetHostName()).FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork)?.ToString();
        public static string IP { get; set; }

        public static string PerfPath { get; set; }

        public static long LimitTime { get; set; } = 100;

        public static bool MonitSql { get; set; }
    }
}