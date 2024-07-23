using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CookingHelper.Server.Migrations
{
    /// <inheritdoc />
    public partial class storeItemUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StoreItem_StoreList_StoreItemGroupId",
                table: "StoreItem");

            migrationBuilder.RenameColumn(
                name: "StoreItemGroupId",
                table: "StoreItem",
                newName: "StoreItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_StoreItem_StoreList_StoreItemId",
                table: "StoreItem",
                column: "StoreItemId",
                principalTable: "StoreList",
                principalColumn: "StoreListId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StoreItem_StoreList_StoreItemId",
                table: "StoreItem");

            migrationBuilder.RenameColumn(
                name: "StoreItemId",
                table: "StoreItem",
                newName: "StoreItemGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_StoreItem_StoreList_StoreItemGroupId",
                table: "StoreItem",
                column: "StoreItemGroupId",
                principalTable: "StoreList",
                principalColumn: "StoreListId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
