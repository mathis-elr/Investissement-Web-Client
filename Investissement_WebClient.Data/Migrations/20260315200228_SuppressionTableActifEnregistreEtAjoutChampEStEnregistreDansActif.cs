using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Investissement_WebClient.Data.Migrations
{
    /// <inheritdoc />
    public partial class SuppressionTableActifEnregistreEtAjoutChampEStEnregistreDansActif : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompositionModeles_ActifEnregistres_IdActifEnregistre",
                table: "CompositionModeles");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_ActifEnregistres_IdActifEnregistre",
                table: "Transactions");

            migrationBuilder.DropTable(
                name: "ActifEnregistres");

            migrationBuilder.AlterColumn<string>(
                name: "Isin",
                table: "Actifs",
                type: "nchar(12)",
                fixedLength: true,
                maxLength: 12,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(12)",
                oldMaxLength: 12,
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EstEnregistre",
                table: "Actifs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_CompositionModeles_Actifs_IdActifEnregistre",
                table: "CompositionModeles",
                column: "IdActifEnregistre",
                principalTable: "Actifs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Actifs_IdActifEnregistre",
                table: "Transactions",
                column: "IdActifEnregistre",
                principalTable: "Actifs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompositionModeles_Actifs_IdActifEnregistre",
                table: "CompositionModeles");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Actifs_IdActifEnregistre",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "EstEnregistre",
                table: "Actifs");

            migrationBuilder.AlterColumn<string>(
                name: "Isin",
                table: "Actifs",
                type: "nvarchar(12)",
                maxLength: 12,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nchar(12)",
                oldFixedLength: true,
                oldMaxLength: 12,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "ActifEnregistres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Isin = table.Column<string>(type: "nchar(12)", fixedLength: true, maxLength: 12, nullable: true),
                    Nom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Risque = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Symbole = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActifEnregistres", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActifEnregistres_Isin",
                table: "ActifEnregistres",
                column: "Isin",
                unique: true,
                filter: "[Isin] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_CompositionModeles_ActifEnregistres_IdActifEnregistre",
                table: "CompositionModeles",
                column: "IdActifEnregistre",
                principalTable: "ActifEnregistres",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_ActifEnregistres_IdActifEnregistre",
                table: "Transactions",
                column: "IdActifEnregistre",
                principalTable: "ActifEnregistres",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
