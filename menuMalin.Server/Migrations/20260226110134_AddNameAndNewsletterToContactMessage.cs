using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace menuMalin.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddNameAndNewsletterToContactMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ContactMessages",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "SubscribeNewsletter",
                table: "ContactMessages",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "ContactMessages");

            migrationBuilder.DropColumn(
                name: "SubscribeNewsletter",
                table: "ContactMessages");
        }
    }
}
