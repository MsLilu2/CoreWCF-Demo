﻿using CoreWCF.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Net;
using System.Threading.Tasks;

namespace NetCoreServer
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            using (IHost host1 = CreateHostBuilder().Build())
            //using(IHost host2 = CreateHostBuilder1().Build())
            {
                if (args?.Length > 0 && args.ToString() == "-debug")
                {
                    host1.Start();
                    //host2.Start();
                    host1.WaitForShutdown();
                    //host2.WaitForShutdown();
                }
                else
                {
                    await Task.WhenAny(
                    host1.RunAsync()//,
                    //host2.RunAsync()
                    );
                }
            }
        }

        // Listen on 8088 for http, and 8443 for https, 8089 for NetTcp.
        public static IHostBuilder CreateHostBuilder()
        {
            var host = Host.CreateDefaultBuilder()
                    .UseWindowsService()
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder.UseNetTcp(IPAddress.IPv6Any, Startup1.NETTCP_PORT)
                                  .UseStartup<Startup1>();
                    });

            return host;
        }

        public static IHostBuilder CreateHostBuilder1()
        {
            var host = Host.CreateDefaultBuilder()
                           .ConfigureWebHostDefaults(webBuilder =>
                           {
                               webBuilder.UseNetTcp(IPAddress.IPv6Any, Startup2.NETTCP_PORT)
                                         .UseStartup<Startup2>();
                           });

            return host;
        }
    }
}
