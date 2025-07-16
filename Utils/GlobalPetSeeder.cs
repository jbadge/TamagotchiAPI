// Utils/GlobalPetSeeder.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TamagotchiAPI.Models;

namespace TamagotchiAPI.Utils
{
    public static class GlobalPetSeeder
    {
        public static async Task SeedIfMissing(DatabaseContext db)
        {
            var existing = await db.Pets
                .Where(p => p.VisitorId == null)
                .Select(p => p.Name)
                .ToListAsync();

            var missing = GetGlobalPets()
                .Where(p => !existing.Contains(p.Name))
                .ToList();

            if (!missing.Any()) return;

            db.Pets.AddRange(missing);
            await db.SaveChangesAsync();
        }

        private static List<Pet> GetGlobalPets()
        {
            var now = DateTime.UtcNow;

            return new List<Pet>
            {
                new Pet {
                    Name = "Bulbasaur",
                    Birthday = now,
                    HungerLevel = 0,
                    HappinessLevel = 0,
                    LastInteractedWithDate = now,
                    IsDead = false,
                    ImageUrl = "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/other/official-artwork/1.png",
                    SpriteUrl = "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/versions/generation-vi/omegaruby-alphasapphire/1.png",
                    VisitorId = null
                },
                new Pet {
                    Name = "Charmander",
                    Birthday = now,
                    HungerLevel = 0,
                    HappinessLevel = 0,
                    LastInteractedWithDate = now,
                    IsDead = false,
                    ImageUrl = "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/other/official-artwork/4.png",
                    SpriteUrl = "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/versions/generation-vi/omegaruby-alphasapphire/4.png",
                    VisitorId = null
                },
                new Pet {
                    Name = "Squirtle",
                    Birthday = now,
                    HungerLevel = 0,
                    HappinessLevel = 0,
                    LastInteractedWithDate = now,
                    IsDead = false,
                    ImageUrl = "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/other/official-artwork/7.png",
                    SpriteUrl = "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/versions/generation-vi/omegaruby-alphasapphire/7.png",
                    VisitorId = null
                },
                new Pet {
                    Name = "Caterpie",
                    Birthday = now,
                    HungerLevel = 0,
                    HappinessLevel = 0,
                    LastInteractedWithDate = now,
                    IsDead = false,
                    ImageUrl = "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/other/official-artwork/10.png",
                    SpriteUrl = "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/versions/generation-vi/omegaruby-alphasapphire/10.png",
                    VisitorId = null
                },
                new Pet {
                    Name = "Pikachu",
                    Birthday = now,
                    HungerLevel = 0,
                    HappinessLevel = 0,
                    LastInteractedWithDate = now,
                    IsDead = false,
                    ImageUrl = "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/other/official-artwork/25.png",
                    SpriteUrl = "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/versions/generation-vi/omegaruby-alphasapphire/25.png",
                    VisitorId = null
                },
                new Pet {
                    Name = "Eevee",
                    Birthday = now,
                    HungerLevel = 0,
                    HappinessLevel = 0,
                    LastInteractedWithDate = now,
                    IsDead = false,
                    ImageUrl = "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/other/official-artwork/133.png",
                    SpriteUrl = "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/versions/generation-vi/omegaruby-alphasapphire/133.png",
                    VisitorId = null
                }
            };
        }
    }
}
