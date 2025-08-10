using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace TamagotchiAPI.Models
{
    public partial class DatabaseContext : DbContext
    {
        private static string DEVELOPMENT_DATABASE_NAME = "TamagotchiAPIDatabase";

        // Change to true to have logging of SQL statements in development
        private static bool LOG_SQL_STATEMENTS_IN_DEVELOPMENT = false;

        // Database tables
        public DbSet<Pet> Pets { get; set; }
        public DbSet<Playtime> Playtimes { get; set; }
        public DbSet<Feeding> Feedings { get; set; }
        public DbSet<Scolding> Scoldings { get; set; }
        public string VisitorId { get; set; }

        public IQueryable<Pet> GetVisitorPets() =>
            Pets.Where(p => p.VisitorId == VisitorId);

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
            if (connection.Contains("pooler.supabase.com"))
            {
                return connection;
            }

            var uri = new Uri(connection);

            var userInfoParts = uri.UserInfo.Split(':');
            var user = userInfoParts[0];
            var password = userInfoParts.Length > 1 ? userInfoParts[1] : "";

            var builder = new Npgsql.NpgsqlConnectionStringBuilder
            {
                Host = uri.Host,
                Port = uri.Port,
                Username = user,
                Password = password,
                Database = uri.AbsolutePath.TrimStart('/'),
                SslMode = Npgsql.SslMode.Require,
                TrustServerCertificate = true
            };

            return builder.ConnectionString;
        }
    }
}