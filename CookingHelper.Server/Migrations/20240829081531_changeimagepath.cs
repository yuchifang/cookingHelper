using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CookingHelper.Server.Migrations
{
    /// <inheritdoc />
#pragma warning disable CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
    public partial class changeimagepath : Migration
#pragma warning restore CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "ImageContent", table: "RecipeItem");

            migrationBuilder
                .AddColumn<string>(
                    name: "ImagePath",
                    table: "RecipeItem",
                    type: "longtext",
                    nullable: false
                )
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "ImagePath", table: "RecipeItem");

            migrationBuilder.AddColumn<byte[]>(
                name: "ImageContent",
                table: "RecipeItem",
                type: "longblob",
                nullable: false
            );
        }
    }
}
