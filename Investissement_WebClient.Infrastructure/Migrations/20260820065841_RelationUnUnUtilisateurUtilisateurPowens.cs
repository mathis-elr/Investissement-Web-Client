using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Investissement_WebClient.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RelationUnUnUtilisateurUtilisateurPowens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UtilisateurPowens_UtilisateurId",
                table: "UtilisateurPowens");

            migrationBuilder.CreateIndex(
                name: "IX_UtilisateurPowens_UtilisateurId",
                table: "UtilisateurPowens",
                column: "UtilisateurId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UtilisateurPowens_UtilisateurId",
                table: "UtilisateurPowens");

            migrationBuilder.CreateIndex(
                name: "IX_UtilisateurPowens_UtilisateurId",
                table: "UtilisateurPowens",
                column: "UtilisateurId");
        }
    }
}
