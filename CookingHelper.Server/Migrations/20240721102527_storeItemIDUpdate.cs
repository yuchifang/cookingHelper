using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CookingHelper.Server.Migrations
{
    /// <inheritdoc />
    public partial class storeItemIDUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StoreItem_StoreList_StoreItemId",
                table: "StoreItem");

            migrationBuilder.AlterColumn<int>(
                name: "StoreItemId",
                table: "StoreItem",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.CreateIndex(
                name: "IX_StoreItem_StoreListId",
                table: "StoreItem",
                column: "StoreListId");

            migrationBuilder.AddForeignKey(
                name: "FK_StoreItem_StoreList_StoreListId",
                table: "StoreItem",
                column: "StoreListId",
                principalTable: "StoreList",
                principalColumn: "StoreListId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StoreItem_StoreList_StoreListId",
                table: "StoreItem");

            migrationBuilder.DropIndex(
                name: "IX_StoreItem_StoreListId",
                table: "StoreItem");

            migrationBuilder.AlterColumn<int>(
                name: "StoreItemId",
                table: "StoreItem",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddForeignKey(
                name: "FK_StoreItem_StoreList_StoreItemId",
                table: "StoreItem",
                column: "StoreItemId",
                principalTable: "StoreList",
                principalColumn: "StoreListId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
