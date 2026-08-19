using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Investissement_WebClient.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NouvelleTableCompteBanque_UpdateAccesBanque : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IdCompteCourant",
                table: "BanqueAcces",
                newName: "IdBanque");

            migrationBuilder.AddColumn<string>(
                name: "NomBanque",
                table: "BanqueAcces",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "CompteBanque",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCompte = table.Column<int>(type: "int", nullable: false),
                    Nom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BanqueAccesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompteBanque", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompteBanque_BanqueAcces_BanqueAccesId",
                        column: x => x.BanqueAccesId,
                        principalTable: "BanqueAcces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompteBanque_BanqueAccesId",
                table: "CompteBanque",
                column: "BanqueAccesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompteBanque");

            migrationBuilder.DropColumn(
                name: "NomBanque",
                table: "BanqueAcces");

            migrationBuilder.RenameColumn(
                name: "IdBanque",
                table: "BanqueAcces",
                newName: "IdCompteCourant");
        }
    }
}
