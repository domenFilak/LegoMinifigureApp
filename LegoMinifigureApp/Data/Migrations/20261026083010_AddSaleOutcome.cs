using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegoMinifigureApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSaleOutcome : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ArchiveNote",
                table: "Minifigures",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SalePrice",
                table: "Minifigures",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Minifigures_ArchiveNote",
                table: "Minifigures",
                sql: "[ArchiveNote] IS NULL OR [Status] <> 'Owned'");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Minifigures_SalePrice",
                table: "Minifigures",
                sql: "[SalePrice] IS NULL OR ([Status] = 'Sold' AND [SalePrice] > 0)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Minifigures_ArchiveNote",
                table: "Minifigures");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Minifigures_SalePrice",
                table: "Minifigures");

            migrationBuilder.DropColumn(
                name: "ArchiveNote",
                table: "Minifigures");

            migrationBuilder.DropColumn(
                name: "SalePrice",
                table: "Minifigures");
        }
    }
}
