using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Investissement_WebClient.Data.Migrations
{
    /// <inheritdoc />
    public partial class CleanCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CategorieFlux",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "CategorieFlux",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "CategorieFlux",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "CategorieFlux",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "CategorieFlux",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "CategorieFlux",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "CategorieFlux",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "CategorieFlux",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "CategorieFlux",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "CategorieFlux",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "CategorieFlux",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "CategorieFlux",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "CategorieFlux",
                keyColumn: "Id",
                keyValue: 13);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "CategorieFlux",
                columns: new[] { "Id", "Libelle" },
                values: new object[,]
                {
                    { 1, "Alimentation" },
                    { 2, "Transport" },
                    { 3, "Virement emis" },
                    { 4, "Virement reçu" },
                    { 5, "Virement emis livret A" },
                    { 6, "Virement reçu livret A" },
                    { 7, "Shopping" },
                    { 8, "Sport" },
                    { 9, "Salaire" },
                    { 10, "APL" },
                    { 11, "Autre" },
                    { 12, "Investissement" },
                    { 13, "Abonnement" }
                });
        }
    }
}
