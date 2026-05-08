using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Investissement_WebClient.Data.Migrations
{
    /// <inheritdoc />
    public partial class AjoutFluxCreditCoop : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FluxBancaires");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Transactions",
                table: "Transactions");

            migrationBuilder.RenameTable(
                name: "Transactions",
                newName: "Transaction");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Transaction",
                table: "Transaction",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "CategorieFlux",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Libelle = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategorieFlux", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CreditCoopAcces",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccesToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditCoopAcces", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FluxTradeRepublic",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Date = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Expediteur = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Montant = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FluxTradeRepublic", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FluxCreditCoop",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Valeur = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LibelleRecu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LibelleSupplementaire = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CategorieId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FluxCreditCoop", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FluxCreditCoop_CategorieFlux_CategorieId",
                        column: x => x.CategorieId,
                        principalTable: "CategorieFlux",
                        principalColumn: "Id");
                });

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
                    { 11, "Autre" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_FluxCreditCoop_CategorieId",
                table: "FluxCreditCoop",
                column: "CategorieId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CreditCoopAcces");

            migrationBuilder.DropTable(
                name: "FluxCreditCoop");

            migrationBuilder.DropTable(
                name: "FluxTradeRepublic");

            migrationBuilder.DropTable(
                name: "CategorieFlux");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Transaction",
                table: "Transaction");

            migrationBuilder.RenameTable(
                name: "Transaction",
                newName: "Transactions");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Transactions",
                table: "Transactions",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "FluxBancaires",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Date = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Expediteur = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Montant = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FluxBancaires", x => x.Id);
                });
        }
    }
}
