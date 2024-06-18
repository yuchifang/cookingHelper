using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CookingHelper.Server.Migrations
{
    /// <inheritdoc />
    public partial class changeName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ShoppingListModel",
                table: "ShoppingListModel");

            migrationBuilder.RenameTable(
                name: "ShoppingListModel",
                newName: "UserList");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserList",
                table: "UserList",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserList",
                table: "UserList");

            migrationBuilder.RenameTable(
                name: "UserList",
                newName: "ShoppingListModel");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShoppingListModel",
                table: "ShoppingListModel",
                column: "UserId");
        }
    }
}
