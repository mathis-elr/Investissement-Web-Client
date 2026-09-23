using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Investissement_WebClient.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AjoutTablePostionInvestissement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PositionInvestissement",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActifId = table.Column<int>(type: "int", nullable: false),
                    Quantite = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PrixAchat = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DateCours = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DerniereMaj = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompteInvestissementId = table.Column<int>(type: "int", nullable: false),
                    CompteBanqueId = table.Column<int>(type: "int", nullable: true),
                    UtilisateurId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PositionInvestissement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PositionInvestissement_Actif_ActifId",
                        column: x => x.ActifId,
                        principalTable: "Actif",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PositionInvestissement_CompteBanque_CompteBanqueId",
                        column: x => x.CompteBanqueId,
                        principalTable: "CompteBanque",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PositionInvestissement_Utilisateur_UtilisateurId",
                        column: x => x.UtilisateurId,
                        principalTable: "Utilisateur",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PositionInvestissement_ActifId",
                table: "PositionInvestissement",
                column: "ActifId");

            migrationBuilder.CreateIndex(
                name: "IX_PositionInvestissement_CompteBanqueId",
                table: "PositionInvestissement",
                column: "CompteBanqueId");

            migrationBuilder.CreateIndex(
                name: "IX_PositionInvestissement_UtilisateurId",
                table: "PositionInvestissement",
                column: "UtilisateurId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PositionInvestissement");
        }
    }
}
