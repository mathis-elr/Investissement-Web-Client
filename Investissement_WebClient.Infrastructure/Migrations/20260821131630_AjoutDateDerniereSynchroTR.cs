using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Investissement_WebClient.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AjoutDateDerniereSynchroTR : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TradeRepublicAcces_Utilisateur_UtilisateurId",
                table: "TradeRepublicAcces");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TradeRepublicAcces",
                table: "TradeRepublicAcces");

            migrationBuilder.RenameTable(
                name: "TradeRepublicAcces",
                newName: "CompteTradeRepublic");

            migrationBuilder.RenameIndex(
                name: "IX_TradeRepublicAcces_UtilisateurId",
                table: "CompteTradeRepublic",
                newName: "IX_CompteTradeRepublic_UtilisateurId");

            migrationBuilder.AddColumn<DateTime>(
                name: "DerniereSynchronisation",
                table: "CompteTradeRepublic",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompteTradeRepublic",
                table: "CompteTradeRepublic",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CompteTradeRepublic_Utilisateur_UtilisateurId",
                table: "CompteTradeRepublic",
                column: "UtilisateurId",
                principalTable: "Utilisateur",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompteTradeRepublic_Utilisateur_UtilisateurId",
                table: "CompteTradeRepublic");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompteTradeRepublic",
                table: "CompteTradeRepublic");

            migrationBuilder.DropColumn(
                name: "DerniereSynchronisation",
                table: "CompteTradeRepublic");

            migrationBuilder.RenameTable(
                name: "CompteTradeRepublic",
                newName: "TradeRepublicAcces");

            migrationBuilder.RenameIndex(
                name: "IX_CompteTradeRepublic_UtilisateurId",
                table: "TradeRepublicAcces",
                newName: "IX_TradeRepublicAcces_UtilisateurId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TradeRepublicAcces",
                table: "TradeRepublicAcces",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TradeRepublicAcces_Utilisateur_UtilisateurId",
                table: "TradeRepublicAcces",
                column: "UtilisateurId",
                principalTable: "Utilisateur",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
