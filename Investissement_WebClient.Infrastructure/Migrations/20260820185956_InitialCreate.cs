using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Investissement_WebClient.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Actif",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Libelle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ISIN = table.Column<string>(type: "nchar(12)", fixedLength: true, maxLength: 12, nullable: false),
                    Ticker = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actif", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CategorieFlux",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MacroCategorie = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MicroCategorie = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategorieFlux", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Utilisateur",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Prenom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MdpHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateCreationCompte = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utilisateur", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FluxInvestissement",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    ActifId = table.Column<int>(type: "int", nullable: false),
                    Prix = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Quantite = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    Frais = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Total = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    UtilisateurId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FluxInvestissement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FluxInvestissement_Actif_ActifId",
                        column: x => x.ActifId,
                        principalTable: "Actif",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FluxInvestissement_Utilisateur_UtilisateurId",
                        column: x => x.UtilisateurId,
                        principalTable: "Utilisateur",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TradeRepublicAcces",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumTelCrypte = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PinCrypte = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UtilisateurId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradeRepublicAcces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TradeRepublicAcces_Utilisateur_UtilisateurId",
                        column: x => x.UtilisateurId,
                        principalTable: "Utilisateur",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UtilisateurPowens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUtilisateurPowens = table.Column<int>(type: "int", nullable: false),
                    AccessTokenCrypte = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UtilisateurId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UtilisateurPowens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UtilisateurPowens_Utilisateur_UtilisateurId",
                        column: x => x.UtilisateurId,
                        principalTable: "Utilisateur",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ValeurPatrimoine",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InvestissementTotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Valeur = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    UtilisateurId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ValeurPatrimoine", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ValeurPatrimoine_Utilisateur_UtilisateurId",
                        column: x => x.UtilisateurId,
                        principalTable: "Utilisateur",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Banque",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdConnectionPowens = table.Column<int>(type: "int", nullable: false),
                    IdConnectorPowens = table.Column<int>(type: "int", nullable: false),
                    Nom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UtilisateurPowensId = table.Column<int>(type: "int", nullable: false),
                    UtilisateurId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Banque", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Banque_UtilisateurPowens_UtilisateurPowensId",
                        column: x => x.UtilisateurPowensId,
                        principalTable: "UtilisateurPowens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Banque_Utilisateur_UtilisateurId",
                        column: x => x.UtilisateurId,
                        principalTable: "Utilisateur",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CompteBanque",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdComptePowens = table.Column<int>(type: "int", nullable: false),
                    TypePowens = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TypeCompte = table.Column<int>(type: "int", nullable: false),
                    Nom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Solde = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    BanqueId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompteBanque", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompteBanque_Banque_BanqueId",
                        column: x => x.BanqueId,
                        principalTable: "Banque",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FluxBancaire",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Valeur = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Libelle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdCategorie = table.Column<int>(type: "int", nullable: true),
                    Suggestion = table.Column<bool>(type: "bit", nullable: false),
                    CompteBanqueId = table.Column<int>(type: "int", nullable: true),
                    UtilisateurId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FluxBancaire", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FluxBancaire_CategorieFlux_IdCategorie",
                        column: x => x.IdCategorie,
                        principalTable: "CategorieFlux",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FluxBancaire_CompteBanque_CompteBanqueId",
                        column: x => x.CompteBanqueId,
                        principalTable: "CompteBanque",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FluxBancaire_Utilisateur_UtilisateurId",
                        column: x => x.UtilisateurId,
                        principalTable: "Utilisateur",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "CategorieFlux",
                columns: new[] { "Id", "MacroCategorie", "MicroCategorie" },
                values: new object[,]
                {
                    { 1, "Vie quotidienne", "Alimentation" },
                    { 2, "Vie quotidienne", "Transport" },
                    { 3, "Vie quotidienne", "Achat de nécéssité" },
                    { 4, "Vie quotidienne", "Sport" },
                    { 5, "Vie quotidienne", "Abonnement fixe" },
                    { 6, "Vie quotidienne", "Logement" },
                    { 7, "Vie quotidienne", "Santé" },
                    { 8, "Revenus", "Salaire" },
                    { 9, "Revenus", "Aide" },
                    { 10, "Revenus", "Cadeau reçu" },
                    { 11, "Patrimoine", "Investissement TR" },
                    { 12, "Patrimoine", "Investissement AV" },
                    { 13, "Patrimoine", "Epargne" },
                    { 14, "Loisirs/Plaisirs", "Achat plaisir" },
                    { 15, "Loisirs/Plaisirs", "Vacances" },
                    { 16, "Loisirs/Plaisirs", "Abonnement plaisir" },
                    { 17, "Loisirs/Plaisirs", "Achat cadeau" },
                    { 18, null, "Autre" },
                    { 19, "Livret A", "Livret A" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Banque_UtilisateurId",
                table: "Banque",
                column: "UtilisateurId");

            migrationBuilder.CreateIndex(
                name: "IX_Banque_UtilisateurPowensId",
                table: "Banque",
                column: "UtilisateurPowensId");

            migrationBuilder.CreateIndex(
                name: "IX_CompteBanque_BanqueId",
                table: "CompteBanque",
                column: "BanqueId");

            migrationBuilder.CreateIndex(
                name: "IX_FluxBancaire_CompteBanqueId",
                table: "FluxBancaire",
                column: "CompteBanqueId");

            migrationBuilder.CreateIndex(
                name: "IX_FluxBancaire_IdCategorie",
                table: "FluxBancaire",
                column: "IdCategorie");

            migrationBuilder.CreateIndex(
                name: "IX_FluxBancaire_UtilisateurId",
                table: "FluxBancaire",
                column: "UtilisateurId");

            migrationBuilder.CreateIndex(
                name: "IX_FluxInvestissement_ActifId",
                table: "FluxInvestissement",
                column: "ActifId");

            migrationBuilder.CreateIndex(
                name: "IX_FluxInvestissement_UtilisateurId",
                table: "FluxInvestissement",
                column: "UtilisateurId");

            migrationBuilder.CreateIndex(
                name: "IX_TradeRepublicAcces_UtilisateurId",
                table: "TradeRepublicAcces",
                column: "UtilisateurId");

            migrationBuilder.CreateIndex(
                name: "IX_Utilisateur_Email",
                table: "Utilisateur",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UtilisateurPowens_UtilisateurId",
                table: "UtilisateurPowens",
                column: "UtilisateurId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ValeurPatrimoine_UtilisateurId",
                table: "ValeurPatrimoine",
                column: "UtilisateurId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FluxBancaire");

            migrationBuilder.DropTable(
                name: "FluxInvestissement");

            migrationBuilder.DropTable(
                name: "TradeRepublicAcces");

            migrationBuilder.DropTable(
                name: "ValeurPatrimoine");

            migrationBuilder.DropTable(
                name: "CategorieFlux");

            migrationBuilder.DropTable(
                name: "CompteBanque");

            migrationBuilder.DropTable(
                name: "Actif");

            migrationBuilder.DropTable(
                name: "Banque");

            migrationBuilder.DropTable(
                name: "UtilisateurPowens");

            migrationBuilder.DropTable(
                name: "Utilisateur");
        }
    }
}
