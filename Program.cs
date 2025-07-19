using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TamagotchiAPI.Models;
using TamagotchiAPI.Utils;

namespace TamagotchiAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {

            var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";

            var host = Utilities.CreateWebHostBuilder(args)
            // For Render
            .UseUrls($"http://*:{port}")
            // For local
            // .UseUrls("http://*:5000")
            .Build();

            using (var scope = host.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                var canContinue = await Utilities.WaitForMigrations(host, context);

                if (!canContinue)
                {
                    return;
                }
            }

            Utilities.Notify("TamagotchiAPI Running!");

            await host.StartAsync();

            await host.WaitForShutdownAsync();

        }
    }
}
