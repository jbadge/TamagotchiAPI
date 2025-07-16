using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TamagotchiAPI.Migrations
{
    public partial class SeedGlobalPets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Pets",
                columns: new[] { "Id", "Birthday", "CreatedAt", "HappinessLevel", "HungerLevel", "ImageUrl", "IsDead", "LastInteractedWithDate", "Name", "SpriteUrl", "VisitorId" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 7, 16, 1, 59, 58, 484, DateTimeKind.Utc).AddTicks(5240), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 0, "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/other/official-artwork/1.png", false, new DateTime(2025, 7, 16, 1, 59, 58, 484, DateTimeKind.Utc).AddTicks(5240), "Bulbasaur", "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/versions/generation-vi/omegaruby-alphasapphire/1.png", null },
                    { 2, new DateTime(2025, 7, 16, 1, 59, 58, 484, DateTimeKind.Utc).AddTicks(5250), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 0, "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/other/official-artwork/4.png", false, new DateTime(2025, 7, 16, 1, 59, 58, 484, DateTimeKind.Utc).AddTicks(5250), "Charmander", "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/versions/generation-vi/omegaruby-alphasapphire/4.png", null },
                    { 3, new DateTime(2025, 7, 16, 1, 59, 58, 484, DateTimeKind.Utc).AddTicks(5250), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 0, "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/other/official-artwork/7.png", false, new DateTime(2025, 7, 16, 1, 59, 58, 484, DateTimeKind.Utc).AddTicks(5250), "Squirtle", "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/versions/generation-vi/omegaruby-alphasapphire/7.png", null },
                    { 4, new DateTime(2025, 7, 16, 1, 59, 58, 484, DateTimeKind.Utc).AddTicks(5250), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 0, "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/other/official-artwork/10.png", false, new DateTime(2025, 7, 16, 1, 59, 58, 484, DateTimeKind.Utc).AddTicks(5250), "Caterpie", "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/versions/generation-vi/omegaruby-alphasapphire/10.png", null },
                    { 5, new DateTime(2025, 7, 16, 1, 59, 58, 484, DateTimeKind.Utc).AddTicks(5250), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 0, "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/other/official-artwork/25.png", false, new DateTime(2025, 7, 16, 1, 59, 58, 484, DateTimeKind.Utc).AddTicks(5250), "Pikachu", "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/versions/generation-vi/omegaruby-alphasapphire/25.png", null },
                    { 6, new DateTime(2025, 7, 16, 1, 59, 58, 484, DateTimeKind.Utc).AddTicks(5260), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 0, "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/other/official-artwork/133.png", false, new DateTime(2025, 7, 16, 1, 59, 58, 484, DateTimeKind.Utc).AddTicks(5260), "Eevee", "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/versions/generation-vi/omegaruby-alphasapphire/133.png", null }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Pets",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Pets",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Pets",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Pets",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Pets",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Pets",
                keyColumn: "Id",
                keyValue: 6);
        }
    }
}
