using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Investissement_WebClient.Data.Migrations
{
    /// <inheritdoc />
    public partial class CategorieNonNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FluxCreditCoop_CategorieFlux_CategorieId",
                table: "FluxCreditCoop");

            migrationBuilder.AlterColumn<string>(
                name: "LibelleSupplementaire",
                table: "FluxCreditCoop",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CategorieId",
                table: "FluxCreditCoop",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FluxCreditCoop_CategorieFlux_CategorieId",
                table: "FluxCreditCoop",
                column: "CategorieId",
                principalTable: "CategorieFlux",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FluxCreditCoop_CategorieFlux_CategorieId",
                table: "FluxCreditCoop");

            migrationBuilder.AlterColumn<string>(
                name: "LibelleSupplementaire",
                table: "FluxCreditCoop",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "CategorieId",
                table: "FluxCreditCoop",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_FluxCreditCoop_CategorieFlux_CategorieId",
                table: "FluxCreditCoop",
                column: "CategorieId",
                principalTable: "CategorieFlux",
                principalColumn: "Id");
        }
    }
}
