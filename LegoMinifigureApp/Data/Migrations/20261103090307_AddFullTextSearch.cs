using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegoMinifigureApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFullTextSearch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "CREATE FULLTEXT CATALOG ftMinifigures;",
                suppressTransaction: true);

            migrationBuilder.Sql(
                """
                CREATE FULLTEXT INDEX ON dbo.Minifigures
                (
                    Name LANGUAGE 1033,
                    BrickLinkId LANGUAGE 1033
                )
                KEY INDEX PK_Minifigures
                ON ftMinifigures
                WITH CHANGE_TRACKING = AUTO, STOPLIST = OFF;
                """,
                suppressTransaction: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "DROP FULLTEXT INDEX ON dbo.Minifigures;",
                suppressTransaction: true);

            migrationBuilder.Sql(
                "DROP FULLTEXT CATALOG ftMinifigures;",
                suppressTransaction: true);
        }
    }
}
