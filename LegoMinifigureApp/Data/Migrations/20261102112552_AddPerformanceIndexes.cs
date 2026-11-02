using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegoMinifigureApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPerformanceIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Minifigures_Status",
                table: "Minifigures");

            migrationBuilder.DropIndex(
                name: "IX_Minifigures_ThemeId",
                table: "Minifigures");

            migrationBuilder.CreateIndex(
                name: "IX_Minifigures_Archive",
                table: "Minifigures",
                columns: new[] { "StatusChangedAt", "Id" },
                descending: new bool[0],
                filter: "[IsDeleted] = 0 AND [Status] <> N'Owned'")
                .Annotation("SqlServer:Include", new[] { "Status", "ThemeId", "Name", "BrickLinkId" });

            migrationBuilder.CreateIndex(
                name: "IX_Minifigures_Status_CreatedAt",
                table: "Minifigures",
                columns: new[] { "Status", "CreatedAt", "Id" },
                descending: new[] { false, true, true },
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Minifigures_Status_Name",
                table: "Minifigures",
                columns: new[] { "Status", "Name", "Id" },
                filter: "[IsDeleted] = 0")
                .Annotation("SqlServer:Include", new[] { "ThemeId", "BrickLinkId", "IsForSale", "IsForTrade" });

            migrationBuilder.CreateIndex(
                name: "IX_Minifigures_ThemeId_Status",
                table: "Minifigures",
                columns: new[] { "ThemeId", "Status" },
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Minifigures_Archive",
                table: "Minifigures");

            migrationBuilder.DropIndex(
                name: "IX_Minifigures_Status_CreatedAt",
                table: "Minifigures");

            migrationBuilder.DropIndex(
                name: "IX_Minifigures_Status_Name",
                table: "Minifigures");

            migrationBuilder.DropIndex(
                name: "IX_Minifigures_ThemeId_Status",
                table: "Minifigures");

            migrationBuilder.CreateIndex(
                name: "IX_Minifigures_Status",
                table: "Minifigures",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Minifigures_ThemeId",
                table: "Minifigures",
                column: "ThemeId");
        }
    }
}
