using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Investissement_WebClient.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AccesIdCompteEtDateExpiration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateExpiration",
                table: "CreditCoopAcces",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "IdCompteCourant",
                table: "CreditCoopAcces",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateExpiration",
                table: "CreditCoopAcces");

            migrationBuilder.DropColumn(
                name: "IdCompteCourant",
                table: "CreditCoopAcces");
        }
    }
}
