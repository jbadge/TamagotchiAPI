using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TamagotchiAPI.Migrations
{
    public partial class AddsSpriteUrl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SpriteUrl",
                table: "Pets",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SpriteUrl",
                table: "Pets");
        }
    }
}
