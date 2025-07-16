using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TamagotchiAPI.Migrations
{
    public partial class AddedVisitorId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Pets",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "VisitorId",
                table: "Pets",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Pets");

            migrationBuilder.DropColumn(
                name: "VisitorId",
                table: "Pets");
        }
    }
}
