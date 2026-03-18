using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Investissement_WebClient.Data.Migrations
{
    /// <inheritdoc />
    public partial class AjoutSuppressionRestrictEtCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompositionModeles_Actifs_IdActifEnregistre",
                table: "CompositionModeles");

            migrationBuilder.DropForeignKey(
                name: "FK_Investissements_Modeles_IdModele",
                table: "Investissements");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Actifs_IdActifEnregistre",
                table: "Transactions");

            migrationBuilder.AddForeignKey(
                name: "FK_CompositionModeles_Actifs_IdActifEnregistre",
                table: "CompositionModeles",
                column: "IdActifEnregistre",
                principalTable: "Actifs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Investissements_Modeles_IdModele",
                table: "Investissements",
                column: "IdModele",
                principalTable: "Modeles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Actifs_IdActifEnregistre",
                table: "Transactions",
                column: "IdActifEnregistre",
                principalTable: "Actifs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompositionModeles_Actifs_IdActifEnregistre",
                table: "CompositionModeles");

            migrationBuilder.DropForeignKey(
                name: "FK_Investissements_Modeles_IdModele",
                table: "Investissements");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Actifs_IdActifEnregistre",
                table: "Transactions");

            migrationBuilder.AddForeignKey(
                name: "FK_CompositionModeles_Actifs_IdActifEnregistre",
                table: "CompositionModeles",
                column: "IdActifEnregistre",
                principalTable: "Actifs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Investissements_Modeles_IdModele",
                table: "Investissements",
                column: "IdModele",
                principalTable: "Modeles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Actifs_IdActifEnregistre",
                table: "Transactions",
                column: "IdActifEnregistre",
                principalTable: "Actifs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
