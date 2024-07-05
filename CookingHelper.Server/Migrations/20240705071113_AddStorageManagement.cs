using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CookingHelper.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddStorageManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder
                .CreateTable(
                    name: "StoreList",
                    columns: table => new
                    {
                        StoreListId = table
                            .Column<int>(type: "int", nullable: false)
                            .Annotation(
                                "MySql:ValueGenerationStrategy",
                                MySqlValueGenerationStrategy.IdentityColumn
                            ),
                        UserId = table
                            .Column<string>(type: "varchar(255)", nullable: false)
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
                            onDelete: ReferentialAction.Cascade
                        );
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "StoreItemGroup",
                    columns: table => new
                    {
                        StoreItemGroupId = table.Column<int>(type: "int", nullable: false),
                        Name = table
                            .Column<string>(type: "longtext", nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        Place = table
                            .Column<string>(type: "longtext", nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        Location = table
                            .Column<string>(type: "longtext", nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        PurchaseDate = table.Column<DateOnly>(type: "date", nullable: true),
                        ExpiryDate = table.Column<DateOnly>(type: "date", nullable: true),
                        StoreListId = table.Column<int>(type: "int", nullable: false)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_StoreItemGroup", x => x.StoreItemGroupId);
                        table.ForeignKey(
                            name: "FK_StoreItemGroup_StoreList_StoreItemGroupId",
                            column: x => x.StoreItemGroupId,
                            principalTable: "StoreList",
                            principalColumn: "StoreListId",
                            onDelete: ReferentialAction.Cascade
                        );
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_StoreList_UserId",
                table: "StoreList",
                column: "UserId",
                unique: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "StoreItemGroup");

            migrationBuilder.DropTable(name: "StoreList");

            migrationBuilder
                .CreateTable(
                    name: "FeedbackGroup",
                    columns: table => new
                    {
                        FeedbackGroupId = table
                            .Column<int>(type: "int", nullable: false)
                            .Annotation(
                                "MySql:ValueGenerationStrategy",
                                MySqlValueGenerationStrategy.IdentityColumn
                            ),
                        UserId = table
                            .Column<string>(type: "varchar(255)", nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4")
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_FeedbackGroup", x => x.FeedbackGroupId);
                        table.ForeignKey(
                            name: "FK_FeedbackGroup_UserList_UserId",
                            column: x => x.UserId,
                            principalTable: "UserList",
                            principalColumn: "UserId",
                            onDelete: ReferentialAction.Cascade
                        );
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "OtherSuggestion",
                    columns: table => new
                    {
                        OtherSuggestionId = table
                            .Column<int>(type: "int", nullable: false)
                            .Annotation(
                                "MySql:ValueGenerationStrategy",
                                MySqlValueGenerationStrategy.IdentityColumn
                            ),
                        FeedbackGroupId = table.Column<int>(type: "int", nullable: false)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_OtherSuggestion", x => x.OtherSuggestionId);
                        table.ForeignKey(
                            name: "FK_OtherSuggestion_FeedbackGroup_FeedbackGroupId",
                            column: x => x.FeedbackGroupId,
                            principalTable: "FeedbackGroup",
                            principalColumn: "FeedbackGroupId",
                            onDelete: ReferentialAction.Cascade
                        );
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "QuestionReply",
                    columns: table => new
                    {
                        QuestionReplyId = table
                            .Column<int>(type: "int", nullable: false)
                            .Annotation(
                                "MySql:ValueGenerationStrategy",
                                MySqlValueGenerationStrategy.IdentityColumn
                            ),
                        FeedbackGroupId = table.Column<int>(type: "int", nullable: false)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_QuestionReply", x => x.QuestionReplyId);
                        table.ForeignKey(
                            name: "FK_QuestionReply_FeedbackGroup_FeedbackGroupId",
                            column: x => x.FeedbackGroupId,
                            principalTable: "FeedbackGroup",
                            principalColumn: "FeedbackGroupId",
                            onDelete: ReferentialAction.Cascade
                        );
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "SystemSuggestion",
                    columns: table => new
                    {
                        SystemSuggestionId = table
                            .Column<int>(type: "int", nullable: false)
                            .Annotation(
                                "MySql:ValueGenerationStrategy",
                                MySqlValueGenerationStrategy.IdentityColumn
                            ),
                        FeedbackGroupId = table.Column<int>(type: "int", nullable: false)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_SystemSuggestion", x => x.SystemSuggestionId);
                        table.ForeignKey(
                            name: "FK_SystemSuggestion_FeedbackGroup_FeedbackGroupId",
                            column: x => x.FeedbackGroupId,
                            principalTable: "FeedbackGroup",
                            principalColumn: "FeedbackGroupId",
                            onDelete: ReferentialAction.Cascade
                        );
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
