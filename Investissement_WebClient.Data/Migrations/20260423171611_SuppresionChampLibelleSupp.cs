using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Investissement_WebClient.Data.Migrations
{
    /// <inheritdoc />
    public partial class SuppresionChampLibelleSupp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LibelleSupplementaire",
                table: "FluxCreditCoop");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LibelleSupplementaire",
                table: "FluxCreditCoop",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
