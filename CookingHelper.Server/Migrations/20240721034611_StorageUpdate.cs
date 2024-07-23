using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CookingHelper.Server.Migrations
{
    /// <inheritdoc />
    public partial class StorageUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StoreItemGroup_StoreList_StoreItemGroupId",
                table: "StoreItemGroup");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StoreItemGroup",
                table: "StoreItemGroup");

            migrationBuilder.RenameTable(
                name: "StoreItemGroup",
                newName: "StoreItem");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StoreItem",
                table: "StoreItem",
                column: "StoreItemGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_StoreItem_StoreList_StoreItemGroupId",
                table: "StoreItem",
                column: "StoreItemGroupId",
                principalTable: "StoreList",
                principalColumn: "StoreListId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StoreItem_StoreList_StoreItemGroupId",
                table: "StoreItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StoreItem",
                table: "StoreItem");

            migrationBuilder.RenameTable(
                name: "StoreItem",
                newName: "StoreItemGroup");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StoreItemGroup",
                table: "StoreItemGroup",
                column: "StoreItemGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_StoreItemGroup_StoreList_StoreItemGroupId",
                table: "StoreItemGroup",
                column: "StoreItemGroupId",
                principalTable: "StoreList",
                principalColumn: "StoreListId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
