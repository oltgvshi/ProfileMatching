using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProfileMatching.Migrations
{
    public partial class JobPostUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "JobPosts",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobPosts_UserId",
                table: "JobPosts",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobPosts_AspNetUsers_UserId",
                table: "JobPosts",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobPosts_AspNetUsers_UserId",
                table: "JobPosts");

            migrationBuilder.DropIndex(
                name: "IX_JobPosts_UserId",
                table: "JobPosts");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "JobPosts");
        }
    }
}
