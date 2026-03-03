using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace menuMalin.Server.Migrations
{
    /// <inheritdoc />
    public partial class IncreaseIngredientsFieldSize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "IngredientsJson",
                table: "UserRecipes",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(2000)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "IngredientsJson",
                table: "UserRecipes",
                type: "varchar(2000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
