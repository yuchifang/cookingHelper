using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CookingHelper.Server.Migrations
{
    /// <inheritdoc />
    public partial class changeimagepath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageContent",
                table: "RecipeItem");

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "RecipeItem",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "RecipeItem");

            migrationBuilder.AddColumn<byte[]>(
                name: "ImageContent",
                table: "RecipeItem",
                type: "longblob",
                nullable: false);
        }
    }
}
