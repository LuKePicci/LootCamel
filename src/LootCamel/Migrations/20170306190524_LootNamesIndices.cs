using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LootCamel.Migrations
{
    public partial class LootNamesIndices : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Nickname",
                table: "LootPlayers",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ItemName",
                table: "LootItems",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LootPlayers_Nickname",
                table: "LootPlayers",
                column: "Nickname",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LootItems_ItemName",
                table: "LootItems",
                column: "ItemName",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LootPlayers_Nickname",
                table: "LootPlayers");

            migrationBuilder.DropIndex(
                name: "IX_LootItems_ItemName",
                table: "LootItems");

            migrationBuilder.AlterColumn<string>(
                name: "Nickname",
                table: "LootPlayers",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ItemName",
                table: "LootItems",
                nullable: true);
        }
    }
}
