using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Investissement_WebClient.Data.Migrations
{
    /// <inheritdoc />
    public partial class AjoutIdCategorieNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FluxCreditCoop_CategorieFlux_CategorieId",
                table: "FluxCreditCoop");

            migrationBuilder.DropIndex(
                name: "IX_FluxCreditCoop_CategorieId",
                table: "FluxCreditCoop");

            migrationBuilder.DropColumn(
                name: "CategorieId",
                table: "FluxCreditCoop");

            migrationBuilder.AddColumn<int>(
                name: "IdCategorie",
                table: "FluxCreditCoop",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FluxCreditCoop_IdCategorie",
                table: "FluxCreditCoop",
                column: "IdCategorie");

            migrationBuilder.AddForeignKey(
                name: "FK_FluxCreditCoop_CategorieFlux_IdCategorie",
                table: "FluxCreditCoop",
                column: "IdCategorie",
                principalTable: "CategorieFlux",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FluxCreditCoop_CategorieFlux_IdCategorie",
                table: "FluxCreditCoop");

            migrationBuilder.DropIndex(
                name: "IX_FluxCreditCoop_IdCategorie",
                table: "FluxCreditCoop");

            migrationBuilder.DropColumn(
                name: "IdCategorie",
                table: "FluxCreditCoop");

            migrationBuilder.AddColumn<int>(
                name: "CategorieId",
                table: "FluxCreditCoop",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_FluxCreditCoop_CategorieId",
                table: "FluxCreditCoop",
                column: "CategorieId");

            migrationBuilder.AddForeignKey(
                name: "FK_FluxCreditCoop_CategorieFlux_CategorieId",
                table: "FluxCreditCoop",
                column: "CategorieId",
                principalTable: "CategorieFlux",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
