using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HipChatServer.Migrations
{
    /// <inheritdoc />
    public partial class chatEntityUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChatSummary",
                table: "Chats",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RecentMessageCount",
                table: "Chats",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChatSummary",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "RecentMessageCount",
                table: "Chats");
        }
    }
}
