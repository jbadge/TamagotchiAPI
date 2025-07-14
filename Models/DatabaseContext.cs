using System;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace TamagotchiAPI.Models
{
    public partial class DatabaseContext : DbContext
    {
        // Change this if you want to have a different database name in development
        private static string DEVELOPMENT_DATABASE_NAME = "TamagotchiAPIDatabase";

        // Change this to true if you want to have logging of SQL statements in development
        private static bool LOG_SQL_STATEMENTS_IN_DEVELOPMENT = false;

        // Database tables
        public DbSet<Pet> Pets { get; set; }
        public DbSet<Playtime> Playtimes { get; set; }
        public DbSet<Feeding> Feedings { get; set; }
        public DbSet<Scolding> Scoldings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (LOG_SQL_STATEMENTS_IN_DEVELOPMENT && Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
                optionsBuilder.UseLoggerFactory(loggerFactory);
            }

            if (!optionsBuilder.IsConfigured)
            {
                var databaseURL = Environment.GetEnvironmentVariable("DATABASE_URL");
                var defaultConnectionString = $"server=localhost;database={DEVELOPMENT_DATABASE_NAME}";

                string conn;

                if (!string.IsNullOrEmpty(databaseURL) && (databaseURL.StartsWith("postgres://") || databaseURL.StartsWith("postgresql://")))

                {
                    conn = ConvertPostConnectionToConnectionString(databaseURL);
                }
                else if (!string.IsNullOrEmpty(databaseURL))
                {
                    // DATABASE_URL is already a standard connection string (not URL), so use as is
                    conn = databaseURL;
                }
                else
                {
                    conn = defaultConnectionString;
                }


                optionsBuilder.UseNpgsql(conn);
            }
        }

        private string ConvertPostConnectionToConnectionString(string connection)
        {
            var _connection = connection.Replace("postgres://", "").Replace("postgresql://", "");

            var connectionParts = Regex.Split(_connection, ":|@|/");

            return $"Host={connectionParts[2]};Port={connectionParts[3]};User Id={connectionParts[0]};Password={connectionParts[1]};Database={connectionParts[4]};SSL Mode=Require;Trust Server Certificate=true;HostAddressFamily=InterNetwork";
        }
    }
}
