🚀 ABP GC & Performance Monitor

An open-source GC and performance monitoring plugin for .NET 7/8/9/10 (C#), designed for ABP Framework applications.

It provides real-time GC metrics, API performance insights, and EF Core SQL execution time tracking, with a built-in dashboard — simple, lightweight, and production-friendly.

✨ Features
🧠 GC Monitoring
Gen0 / Gen1 / Gen2 collection counts
GC pause time
Memory allocation tracking
⚡ API Performance Tracking
Request duration
Slow request detection
Endpoint-level statistics
🗄 EF Core SQL Monitoring
SQL execution time tracking
Slow query detection
Helps identify performance bottlenecks
📊 Built-in Dashboard
Real-time metrics visualization
Clear and intuitive UI
🔌 Plug & Play for ABP
Seamless integration with ABP projects
Minimal configuration required
🪶 Lightweight & High Performance
Low overhead
Suitable for production environments
📦 Installation
dotnet add package Your.Package.Name
⚙️ Quick Start (ABP)
1️⃣ Add Module Dependency
[DependsOn(
    typeof(GCPauseMonitorPluginModule) // replace with your module
)]
public class YourProjectModule : AbpModule
{
}
2️⃣ Enable Monitoring

 app.UseGCPerfMonitor();

 app.UseEndpoints(endpoints =>
 { 
     endpoints.MapGcMonitor("ProjectName");
 });

3️⃣ Run Your Project

Open PageDashboard htm

What you get:
⏱ Execution duration per query
🚨 Slow query detection
📈 Aggregated query statistics

Perfect for diagnosing:

N+1 queries
Long-running SQL
Inefficient indexes
📊 Dashboard Preview
GC metrics (Gen0/1/2)
Memory usage trends
API request duration
SQL query performance

(You can add screenshots here)

🛠 Supported Platforms
.NET 7
.NET 8
.NET 9
.NET 10 (preview)
ABP Framework
💡 Use Cases
Production performance monitoring
Debugging memory issues
Identifying slow APIs
Optimizing EF Core queries
🤝 Contributing

Contributions are welcome!

Feel free to submit issues or pull requests to improve this project.

⭐ Star History

If this project helps you, please give it a ⭐ on GitHub!

📄 License

MIT License

<img width="1351" height="576" alt="p1" src="https://github.com/user-attachments/assets/7ec0d4aa-681e-4f64-ae67-162dddf69f3d" />

<img width="1331" height="595" alt="p2" src="https://github.com/user-attachments/assets/94309a9d-6f19-4ec7-bb7e-d2c25fde967f" />
