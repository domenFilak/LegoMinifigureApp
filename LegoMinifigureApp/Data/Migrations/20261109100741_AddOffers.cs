using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegoMinifigureApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOffers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Offers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MinifigureId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    OfferedPrice = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    Message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    AskingPriceAtOffer = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    StatusChangedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Offers", x => x.Id);
                    table.CheckConstraint("CK_Offers_AskingPriceAtOffer", "[AskingPriceAtOffer] IS NULL OR [AskingPriceAtOffer] > 0");
                    table.CheckConstraint("CK_Offers_DeletedAt", "([IsDeleted] = 0 AND [DeletedAt] IS NULL) OR ([IsDeleted] = 1 AND [DeletedAt] IS NOT NULL)");
                    table.CheckConstraint("CK_Offers_OfferedPrice", "([Type] = 'Buy' AND [OfferedPrice] > 0) OR ([Type] = 'Trade' AND [OfferedPrice] IS NULL)");
                    table.CheckConstraint("CK_Offers_StatusChangedAt", "([Status] = 'New' AND [StatusChangedAt] IS NULL) OR ([Status] <> 'New' AND [StatusChangedAt] IS NOT NULL)");
                    table.CheckConstraint("CK_Offers_TradeMessage", "[Type] <> 'Trade' OR ([Message] IS NOT NULL AND LEN([Message]) > 0)");
                    table.ForeignKey(
                        name: "FK_Offers_Minifigures_MinifigureId",
                        column: x => x.MinifigureId,
                        principalTable: "Minifigures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Offers_MinifigureId",
                table: "Offers",
                column: "MinifigureId");

            migrationBuilder.CreateIndex(
                name: "IX_Offers_Status_CreatedAt",
                table: "Offers",
                columns: new[] { "Status", "CreatedAt", "Id" },
                descending: new[] { false, true, true },
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Offers");
        }
    }
}
