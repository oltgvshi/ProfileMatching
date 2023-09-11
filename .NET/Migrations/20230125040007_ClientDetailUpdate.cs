using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProfileMatching.Migrations
{
    public partial class ClientDetailUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "ClientDetails",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientDetails_UserId",
                table: "ClientDetails",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientDetails_AspNetUsers_UserId",
                table: "ClientDetails",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientDetails_AspNetUsers_UserId",
                table: "ClientDetails");

            migrationBuilder.DropIndex(
                name: "IX_ClientDetails_UserId",
                table: "ClientDetails");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ClientDetails");
        }
    }
}
