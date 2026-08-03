using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Investissement_WebClient.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MajMacroCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "CategorieFlux",
                keyColumn: "Id",
                keyValue: 18,
                column: "MacroCategorie",
                value: null);

            migrationBuilder.UpdateData(
                table: "CategorieFlux",
                keyColumn: "Id",
                keyValue: 19,
                column: "MacroCategorie",
                value: "Livret A");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "CategorieFlux",
                keyColumn: "Id",
                keyValue: 18,
                column: "MacroCategorie",
                value: "Autre");

            migrationBuilder.UpdateData(
                table: "CategorieFlux",
                keyColumn: "Id",
                keyValue: 19,
                column: "MacroCategorie",
                value: null);
        }
    }
}
