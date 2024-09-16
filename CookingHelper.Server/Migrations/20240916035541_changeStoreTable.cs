using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CookingHelper.Server.Migrations
{
    /// <inheritdoc />
    public partial class changeStoreTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StoreItem_StoreList_StoreListId",
                table: "StoreItem");

            migrationBuilder.DropTable(
                name: "StoreList");

            migrationBuilder.DropIndex(
                name: "IX_StoreItem_StoreListId",
                table: "StoreItem");

            migrationBuilder.DropColumn(
                name: "StoreListId",
                table: "StoreItem");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "StoreItem",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_StoreItem_UserId",
                table: "StoreItem",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_StoreItem_UserList_UserId",
                table: "StoreItem",
                column: "UserId",
                principalTable: "UserList",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StoreItem_UserList_UserId",
                table: "StoreItem");

            migrationBuilder.DropIndex(
                name: "IX_StoreItem_UserId",
                table: "StoreItem");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "StoreItem");

            migrationBuilder.AddColumn<int>(
                name: "StoreListId",
                table: "StoreItem",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "StoreList",
                columns: table => new
                {
                    StoreListId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreList", x => x.StoreListId);
                    table.ForeignKey(
                        name: "FK_StoreList_UserList_UserId",
                        column: x => x.UserId,
                        principalTable: "UserList",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_StoreItem_StoreListId",
                table: "StoreItem",
                column: "StoreListId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreList_UserId",
                table: "StoreList",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_StoreItem_StoreList_StoreListId",
                table: "StoreItem",
                column: "StoreListId",
                principalTable: "StoreList",
                principalColumn: "StoreListId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
