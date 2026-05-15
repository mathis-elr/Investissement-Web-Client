using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Investissement_WebClient.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PinCrypteIntToString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UtilisateurId",
                table: "ValeurPatrimoine",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "PinCrypte",
                table: "TradeRepublicAcces",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_ValeurPatrimoine_UtilisateurId",
                table: "ValeurPatrimoine",
                column: "UtilisateurId");

            migrationBuilder.AddForeignKey(
                name: "FK_ValeurPatrimoine_Utilisateur_UtilisateurId",
                table: "ValeurPatrimoine",
                column: "UtilisateurId",
                principalTable: "Utilisateur",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ValeurPatrimoine_Utilisateur_UtilisateurId",
                table: "ValeurPatrimoine");

            migrationBuilder.DropIndex(
                name: "IX_ValeurPatrimoine_UtilisateurId",
                table: "ValeurPatrimoine");

            migrationBuilder.DropColumn(
                name: "UtilisateurId",
                table: "ValeurPatrimoine");

            migrationBuilder.AlterColumn<int>(
                name: "PinCrypte",
                table: "TradeRepublicAcces",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
