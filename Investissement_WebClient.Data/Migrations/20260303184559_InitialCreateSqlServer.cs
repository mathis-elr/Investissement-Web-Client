using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Investissement_WebClient.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateSqlServer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActifEnregistres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Symbole = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Isin = table.Column<string>(type: "nchar(12)", fixedLength: true, maxLength: 12, nullable: true),
                    Risque = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActifEnregistres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Actifs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Isin = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: true),
                    Symbole = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Risque = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actifs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HistoriquePatrimoine",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InvestissementTotal = table.Column<double>(type: "float", nullable: false),
                    Valeur = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoriquePatrimoine", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Modeles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modeles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CompositionModeles",
                columns: table => new
                {
                    IdActifEnregistre = table.Column<int>(type: "int", nullable: false),
                    IdModele = table.Column<int>(type: "int", nullable: false),
                    Quantite = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompositionModeles", x => new { x.IdActifEnregistre, x.IdModele });
                    table.ForeignKey(
                        name: "FK_CompositionModeles_ActifEnregistres_IdActifEnregistre",
                        column: x => x.IdActifEnregistre,
                        principalTable: "ActifEnregistres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompositionModeles_Modeles_IdModele",
                        column: x => x.IdModele,
                        principalTable: "Modeles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Investissements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateInvest = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModele = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Investissements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Investissements_Modeles_IdModele",
                        column: x => x.IdModele,
                        principalTable: "Modeles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantite = table.Column<double>(type: "float", nullable: false),
                    Prix = table.Column<double>(type: "float", nullable: false),
                    Frais = table.Column<double>(type: "float", nullable: true),
                    IdActifEnregistre = table.Column<int>(type: "int", nullable: false),
                    IdInvestissement = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_ActifEnregistres_IdActifEnregistre",
                        column: x => x.IdActifEnregistre,
                        principalTable: "ActifEnregistres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transactions_Investissements_IdInvestissement",
                        column: x => x.IdInvestissement,
                        principalTable: "Investissements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActifEnregistres_Isin",
                table: "ActifEnregistres",
                column: "Isin",
                unique: true,
                filter: "[Isin] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Actifs_Isin",
                table: "Actifs",
                column: "Isin",
                unique: true,
                filter: "[Isin] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CompositionModeles_IdModele",
                table: "CompositionModeles",
                column: "IdModele");

            migrationBuilder.CreateIndex(
                name: "IX_Investissements_IdModele",
                table: "Investissements",
                column: "IdModele");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_IdActifEnregistre",
                table: "Transactions",
                column: "IdActifEnregistre");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_IdInvestissement",
                table: "Transactions",
                column: "IdInvestissement");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Actifs");

            migrationBuilder.DropTable(
                name: "CompositionModeles");

            migrationBuilder.DropTable(
                name: "HistoriquePatrimoine");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "ActifEnregistres");

            migrationBuilder.DropTable(
                name: "Investissements");

            migrationBuilder.DropTable(
                name: "Modeles");
        }
    }
}
