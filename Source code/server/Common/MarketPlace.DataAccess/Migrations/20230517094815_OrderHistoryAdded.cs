using Microsoft.EntityFrameworkCore.Migrations;
using System.Diagnostics.CodeAnalysis;

#nullable disable

namespace MarketPlace.DataAccess.Migrations
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public partial class OrderHistoryAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "OrderDetails");

            migrationBuilder.CreateTable(
                name: "OrderHistory",
                columns: table => new
                {
                    OrderHistoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderDetailsId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderHistory", x => x.OrderHistoryId);
                    table.ForeignKey(
                        name: "FK_OrderHistory_OrderDetails_OrderDetailsId",
                        column: x => x.OrderDetailsId,
                        principalTable: "OrderDetails",
                        principalColumn: "OrderDetailsId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderHistory_OrderDetailsId",
                table: "OrderHistory",
                column: "OrderDetailsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderHistory");

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "OrderDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "Status",
                table: "OrderDetails",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);
        }
    }
}
