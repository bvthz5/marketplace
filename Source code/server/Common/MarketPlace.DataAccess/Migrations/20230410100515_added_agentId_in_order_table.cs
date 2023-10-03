using Microsoft.EntityFrameworkCore.Migrations;
using System.Diagnostics.CodeAnalysis;

#nullable disable

namespace MarketPlace.DataAccess.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class added_agentId_in_order_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AgentId",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_AgentId",
                table: "Orders",
                column: "AgentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Agents_AgentId",
                table: "Orders",
                column: "AgentId",
                principalTable: "Agents",
                principalColumn: "AgentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Agents_AgentId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_AgentId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "AgentId",
                table: "Orders");
        }
    }
}
