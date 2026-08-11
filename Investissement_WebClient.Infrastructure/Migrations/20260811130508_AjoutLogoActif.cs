using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Investissement_WebClient.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AjoutLogoActif : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Logo",
                table: "Actif",
                type: "varbinary(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Logo",
                table: "Actif");
        }
    }
}
