using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Investissement_WebClient.Data.Migrations
{
    /// <inheritdoc />
    public partial class AjoutCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "CategorieFlux",
                columns: new[] { "Id", "Libelle" },
                values: new object[,]
                {
                    { 1, "Alimentation" },
                    { 2, "Transport" },
                    { 3, "Avance Livret A" },
                    { 4, "Dette Livret A" },
                    { 5, "Shopping" },
                    { 6, "Sport" },
                    { 7, "Salaire" },
                    { 8, "APL" },
                    { 9, "Autre" },
                    { 10, "Investissement Trade Republic" },
                    { 11, "Abonnement" },
                    { 12, "Logement" },
                    { 13, "Cadeaux" },
                    { 14, "Achat plaisir" },
                    { 15, "Investissement AV" },
                    { 16, "Vacances" },
                    { 17, "Santé" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DeleteData(
                table: "CategorieFlux",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "CategorieFlux",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "CategorieFlux",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "CategorieFlux",
                keyColumn: "Id",
                keyValue: 17);
        }
    }
}
