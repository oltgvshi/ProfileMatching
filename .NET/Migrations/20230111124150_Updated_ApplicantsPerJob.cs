using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProfileMatching.Migrations
{
    public partial class Updated_ApplicantsPerJob : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicantsPerJobs_JobPosts_JibPostId",
                table: "ApplicantsPerJobs");

            migrationBuilder.DropIndex(
                name: "IX_ApplicantsPerJobs_JibPostId",
                table: "ApplicantsPerJobs");

            migrationBuilder.DropColumn(
                name: "JibPostId",
                table: "ApplicantsPerJobs");

            migrationBuilder.AlterColumn<int>(
                name: "JobPostId",
                table: "ApplicantsPerJobs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApplicantsPerJobs_JobPostId",
                table: "ApplicantsPerJobs",
                column: "JobPostId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicantsPerJobs_JobPosts_JobPostId",
                table: "ApplicantsPerJobs",
                column: "JobPostId",
                principalTable: "JobPosts",
                principalColumn: "JobPostId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicantsPerJobs_JobPosts_JobPostId",
                table: "ApplicantsPerJobs");

            migrationBuilder.DropIndex(
                name: "IX_ApplicantsPerJobs_JobPostId",
                table: "ApplicantsPerJobs");

            migrationBuilder.AlterColumn<int>(
                name: "JobPostId",
                table: "ApplicantsPerJobs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "JibPostId",
                table: "ApplicantsPerJobs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ApplicantsPerJobs_JibPostId",
                table: "ApplicantsPerJobs",
                column: "JibPostId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicantsPerJobs_JobPosts_JibPostId",
                table: "ApplicantsPerJobs",
                column: "JibPostId",
                principalTable: "JobPosts",
                principalColumn: "JobPostId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
