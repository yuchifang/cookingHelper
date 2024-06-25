using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CookingHelper.Server.Migrations
{
    /// <inheritdoc />
    public partial class FeedbackDbContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FeedbackGroup",
                columns: table => new
                {
                    FeedbackGroupId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
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
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "OtherSuggestion",
                columns: table => new
                {
                    OtherSuggestionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
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
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "QuestionReply",
                columns: table => new
                {
                    QuestionReplyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
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
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SystemSuggestion",
                columns: table => new
                {
                    SystemSuggestionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
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
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FeedbackPost",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PostDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Text = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    QuestionReplyId = table.Column<int>(type: "int", nullable: false),
                    OtherSuggestionId = table.Column<int>(type: "int", nullable: false),
                    SystemSuggestionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedbackPost", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeedbackPost_OtherSuggestion_OtherSuggestionId",
                        column: x => x.OtherSuggestionId,
                        principalTable: "OtherSuggestion",
                        principalColumn: "OtherSuggestionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FeedbackPost_QuestionReply_QuestionReplyId",
                        column: x => x.QuestionReplyId,
                        principalTable: "QuestionReply",
                        principalColumn: "QuestionReplyId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FeedbackPost_SystemSuggestion_SystemSuggestionId",
                        column: x => x.SystemSuggestionId,
                        principalTable: "SystemSuggestion",
                        principalColumn: "SystemSuggestionId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackGroup_UserId",
                table: "FeedbackGroup",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackPost_OtherSuggestionId",
                table: "FeedbackPost",
                column: "OtherSuggestionId");

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackPost_QuestionReplyId",
                table: "FeedbackPost",
                column: "QuestionReplyId");

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackPost_SystemSuggestionId",
                table: "FeedbackPost",
                column: "SystemSuggestionId");

            migrationBuilder.CreateIndex(
                name: "IX_OtherSuggestion_FeedbackGroupId",
                table: "OtherSuggestion",
                column: "FeedbackGroupId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuestionReply_FeedbackGroupId",
                table: "QuestionReply",
                column: "FeedbackGroupId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SystemSuggestion_FeedbackGroupId",
                table: "SystemSuggestion",
                column: "FeedbackGroupId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FeedbackPost");

            migrationBuilder.DropTable(
                name: "OtherSuggestion");

            migrationBuilder.DropTable(
                name: "QuestionReply");

            migrationBuilder.DropTable(
                name: "SystemSuggestion");

            migrationBuilder.DropTable(
                name: "FeedbackGroup");
        }
    }
}
