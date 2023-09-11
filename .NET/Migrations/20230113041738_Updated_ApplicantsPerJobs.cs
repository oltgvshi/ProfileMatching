using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProfileMatching.Migrations
{
    public partial class Updated_ApplicantsPerJobs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ApplicantPerJobId",
                table: "ApplicantsPerJobs",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicantsPerJobs",
                table: "ApplicantsPerJobs",
                column: "ApplicantPerJobId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicantsPerJobs",
                table: "ApplicantsPerJobs");

            migrationBuilder.DropColumn(
                name: "ApplicantPerJobId",
                table: "ApplicantsPerJobs");
        }
    }
}
