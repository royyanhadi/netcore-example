using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            System.Net.ServicePointManager.DefaultConnectionLimit = int.MaxValue;
            System.Net.ServicePointManager.UseNagleAlgorithm = false;
            System.Net.ServicePointManager.ReusePort = true;
            System.Net.ServicePointManager.EnableDnsRoundRobin = true;
            System.Net.ServicePointManager.Expect100Continue = false;
            System.Threading.ThreadPool.GetMaxThreads(out int _, out int completionThreads);
            System.Threading.ThreadPool.SetMinThreads(2000, completionThreads);
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    //webBuilder.UseLibuv();
                    webBuilder.UseKestrel();
                    webBuilder.UseLinuxTransport();
                    webBuilder.ConfigureKestrel((context, serverOptions) =>
                    {
                        serverOptions.AllowSynchronousIO = false;
                        serverOptions.Limits.MaxConcurrentConnections = 10000;
                        serverOptions.Limits.MaxConcurrentUpgradedConnections = 10000;
                        serverOptions.Limits.MaxRequestBodySize = 10 * 1024;
                        serverOptions.Limits.MinRequestBodyDataRate = new MinDataRate(bytesPerSecond: 100, gracePeriod: TimeSpan.FromSeconds(10));
                        serverOptions.Limits.MinResponseDataRate =
                        new MinDataRate(bytesPerSecond: 100, gracePeriod: TimeSpan.FromSeconds(10));             
                    });
                    webBuilder.UseStartup<Startup>();
                });
    }
}
