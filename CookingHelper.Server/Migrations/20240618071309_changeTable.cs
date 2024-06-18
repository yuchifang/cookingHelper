using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CookingHelper.Server.Migrations
{
    /// <inheritdoc />
    public partial class changeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ShoppingListModel",
                newName: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "ShoppingListModel",
                newName: "Id");
        }
    }
}
