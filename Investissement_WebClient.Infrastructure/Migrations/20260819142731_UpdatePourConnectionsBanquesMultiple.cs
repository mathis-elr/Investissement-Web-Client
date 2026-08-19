using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Investissement_WebClient.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePourConnectionsBanquesMultiple : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompteBanque_BanqueAcces_BanqueAccesId",
                table: "CompteBanque");

            migrationBuilder.DropTable(
                name: "BanqueAcces");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "CompteBanque",
                newName: "TypePowens");

            migrationBuilder.RenameColumn(
                name: "IdCompte",
                table: "CompteBanque",
                newName: "IdComptePowens");

            migrationBuilder.RenameColumn(
                name: "BanqueAccesId",
                table: "CompteBanque",
                newName: "BanqueId");

            migrationBuilder.RenameIndex(
                name: "IX_CompteBanque_BanqueAccesId",
                table: "CompteBanque",
                newName: "IX_CompteBanque_BanqueId");

            migrationBuilder.AddColumn<int>(
                name: "CompteBanqueId",
                table: "FluxBancaire",
                type: "int",
                nullable: true);

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
                name: "Banque",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdConnectionPowens = table.Column<int>(type: "int", nullable: false),
                    IdConnectorPowens = table.Column<int>(type: "int", nullable: false),
                    Nom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateExpiration = table.Column<DateTime>(type: "datetime2", nullable: false),
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

            migrationBuilder.CreateIndex(
                name: "IX_FluxBancaire_CompteBanqueId",
                table: "FluxBancaire",
                column: "CompteBanqueId");

            migrationBuilder.CreateIndex(
                name: "IX_Banque_UtilisateurId",
                table: "Banque",
                column: "UtilisateurId");

            migrationBuilder.CreateIndex(
                name: "IX_Banque_UtilisateurPowensId",
                table: "Banque",
                column: "UtilisateurPowensId");

            migrationBuilder.CreateIndex(
                name: "IX_UtilisateurPowens_UtilisateurId",
                table: "UtilisateurPowens",
                column: "UtilisateurId");

            migrationBuilder.AddForeignKey(
                name: "FK_CompteBanque_Banque_BanqueId",
                table: "CompteBanque",
                column: "BanqueId",
                principalTable: "Banque",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FluxBancaire_CompteBanque_CompteBanqueId",
                table: "FluxBancaire",
                column: "CompteBanqueId",
                principalTable: "CompteBanque",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompteBanque_Banque_BanqueId",
                table: "CompteBanque");

            migrationBuilder.DropForeignKey(
                name: "FK_FluxBancaire_CompteBanque_CompteBanqueId",
                table: "FluxBancaire");

            migrationBuilder.DropTable(
                name: "Banque");

            migrationBuilder.DropTable(
                name: "UtilisateurPowens");

            migrationBuilder.DropIndex(
                name: "IX_FluxBancaire_CompteBanqueId",
                table: "FluxBancaire");

            migrationBuilder.DropColumn(
                name: "CompteBanqueId",
                table: "FluxBancaire");

            migrationBuilder.RenameColumn(
                name: "TypePowens",
                table: "CompteBanque",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "IdComptePowens",
                table: "CompteBanque",
                newName: "IdCompte");

            migrationBuilder.RenameColumn(
                name: "BanqueId",
                table: "CompteBanque",
                newName: "BanqueAccesId");

            migrationBuilder.RenameIndex(
                name: "IX_CompteBanque_BanqueId",
                table: "CompteBanque",
                newName: "IX_CompteBanque_BanqueAccesId");

            migrationBuilder.CreateTable(
                name: "BanqueAcces",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UtilisateurId = table.Column<int>(type: "int", nullable: false),
                    AccesTokenCrypte = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateExpiration = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdBanque = table.Column<int>(type: "int", nullable: false),
                    NomBanque = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BanqueAcces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BanqueAcces_Utilisateur_UtilisateurId",
                        column: x => x.UtilisateurId,
                        principalTable: "Utilisateur",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BanqueAcces_UtilisateurId",
                table: "BanqueAcces",
                column: "UtilisateurId");

            migrationBuilder.AddForeignKey(
                name: "FK_CompteBanque_BanqueAcces_BanqueAccesId",
                table: "CompteBanque",
                column: "BanqueAccesId",
                principalTable: "BanqueAcces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
