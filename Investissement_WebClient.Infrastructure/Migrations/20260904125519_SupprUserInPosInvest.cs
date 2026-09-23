using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Investissement_WebClient.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SupprUserInPosInvest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PositionInvestissement_CompteBanque_CompteBanqueId",
                table: "PositionInvestissement");

            migrationBuilder.DropForeignKey(
                name: "FK_PositionInvestissement_Utilisateur_UtilisateurId",
                table: "PositionInvestissement");

            migrationBuilder.DropIndex(
                name: "IX_PositionInvestissement_UtilisateurId",
                table: "PositionInvestissement");

            migrationBuilder.DropColumn(
                name: "CompteInvestissementId",
                table: "PositionInvestissement");

            migrationBuilder.DropColumn(
                name: "UtilisateurId",
                table: "PositionInvestissement");

            migrationBuilder.AlterColumn<int>(
                name: "CompteBanqueId",
                table: "PositionInvestissement",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PositionInvestissement_CompteBanque_CompteBanqueId",
                table: "PositionInvestissement",
                column: "CompteBanqueId",
                principalTable: "CompteBanque",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PositionInvestissement_CompteBanque_CompteBanqueId",
                table: "PositionInvestissement");

            migrationBuilder.AlterColumn<int>(
                name: "CompteBanqueId",
                table: "PositionInvestissement",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "CompteInvestissementId",
                table: "PositionInvestissement",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UtilisateurId",
                table: "PositionInvestissement",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PositionInvestissement_UtilisateurId",
                table: "PositionInvestissement",
                column: "UtilisateurId");

            migrationBuilder.AddForeignKey(
                name: "FK_PositionInvestissement_CompteBanque_CompteBanqueId",
                table: "PositionInvestissement",
                column: "CompteBanqueId",
                principalTable: "CompteBanque",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PositionInvestissement_Utilisateur_UtilisateurId",
                table: "PositionInvestissement",
                column: "UtilisateurId",
                principalTable: "Utilisateur",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
