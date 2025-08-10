using System;
using System.Collections.Generic;
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
            var port = Environment.GetEnvironmentVariable("PORT") ?? "5001";
            var URLs = new Dictionary<int, string>
            {
            // // Render
            { 0, $"http://*:{Environment.GetEnvironmentVariable("PORT") ?? "5001"}" }, 
            // // Local Network
            { 1, "http://192.168.0.241:5000" }, 
            // // Local Dev
            { 2, "http://localhost:5001" }
            };

            var MODE = 0;
            var selectedUrl = URLs[MODE];

            var host = Utilities.CreateWebHostBuilder(args)
            .UseUrls(selectedUrl)
            .Build();

            using (var scope = host.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                var canContinue = await Utilities.WaitForMigrations(host, context);

                if (!canContinue)
                {
                    return;
                }

                await GlobalPetSeeder.SeedIfMissing(context);
            }

            Utilities.Notify("TamagotchiAPI Running!");

            await host.StartAsync();

            await host.WaitForShutdownAsync();

        }
    }
}
