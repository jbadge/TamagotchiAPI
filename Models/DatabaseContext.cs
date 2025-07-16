using System;
using System.Linq;
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
        public string VisitorId { get; set; }

        public IQueryable<Pet> GetVisitorPets() =>
            Pets.Where(p => p.VisitorId == VisitorId);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Pet>().HasData(
                new Pet
                {
                    Id = 1,
                    Name = "Bulbasaur",
                    Birthday = DateTime.UtcNow,
                    HungerLevel = 0,
                    HappinessLevel = 0,
                    LastInteractedWithDate = DateTime.UtcNow,
                    IsDead = false,
                    ImageUrl = "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/other/official-artwork/1.png",
                    SpriteUrl = "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/versions/generation-vi/omegaruby-alphasapphire/1.png",
                    VisitorId = null
                },
                new Pet
                {
                    Id = 2,
                    Name = "Charmander",
                    Birthday = DateTime.UtcNow,
                    HungerLevel = 0,
                    HappinessLevel = 0,
                    LastInteractedWithDate = DateTime.UtcNow,
                    IsDead = false,
                    ImageUrl = "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/other/official-artwork/4.png",
                    SpriteUrl = "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/versions/generation-vi/omegaruby-alphasapphire/4.png",
                    VisitorId = null
                },
                new Pet
                {
                    Id = 3,
                    Name = "Squirtle",
                    Birthday = DateTime.UtcNow,
                    HungerLevel = 0,
                    HappinessLevel = 0,
                    LastInteractedWithDate = DateTime.UtcNow,
                    IsDead = false,
                    ImageUrl = "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/other/official-artwork/7.png",
                    SpriteUrl = "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/versions/generation-vi/omegaruby-alphasapphire/7.png",
                    VisitorId = null
                },
                new Pet
                {
                    Id = 4,
                    Name = "Caterpie",
                    Birthday = DateTime.UtcNow,
                    HungerLevel = 0,
                    HappinessLevel = 0,
                    LastInteractedWithDate = DateTime.UtcNow,
                    IsDead = false,
                    ImageUrl = "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/other/official-artwork/10.png",
                    SpriteUrl = "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/versions/generation-vi/omegaruby-alphasapphire/10.png",
                    VisitorId = null
                },
                new Pet
                {
                    Id = 5,
                    Name = "Pikachu",
                    Birthday = DateTime.UtcNow,
                    HungerLevel = 0,
                    HappinessLevel = 0,
                    LastInteractedWithDate = DateTime.UtcNow,
                    IsDead = false,
                    ImageUrl = "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/other/official-artwork/25.png",
                    SpriteUrl = "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/versions/generation-vi/omegaruby-alphasapphire/25.png",
                    VisitorId = null
                },
                new Pet
                {
                    Id = 6,
                    Name = "Eevee",
                    Birthday = DateTime.UtcNow,
                    HungerLevel = 0,
                    HappinessLevel = 0,
                    LastInteractedWithDate = DateTime.UtcNow,
                    IsDead = false,
                    ImageUrl = "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/other/official-artwork/133.png",
                    SpriteUrl = "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/versions/generation-vi/omegaruby-alphasapphire/133.png",
                    VisitorId = null
                }
            );
        }

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
            if (connection.Contains("pooler.supabase.com"))
            {
                // Return as-is for pooler (might want to add SSL Mode if needed)
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