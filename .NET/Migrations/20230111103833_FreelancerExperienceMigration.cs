using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProfileMatching.Migrations
{
    public partial class FreelancerExperience : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FreelancerDetails_Categories_CategoryId",
                table: "FreelancerDetails");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "FreelancerDetails",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "FreelancerExperiences",
                columns: table => new
                {
                    FreelancerExperienceID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    EmploymentTypeId = table.Column<int>(type: "int", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FreelancerExperiences", x => x.FreelancerExperienceID);
                    table.ForeignKey(
                        name: "FK_FreelancerExperiences_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FreelancerExperiences_EmploymentTypes_EmploymentTypeId",
                        column: x => x.EmploymentTypeId,
                        principalTable: "EmploymentTypes",
                        principalColumn: "EmploymentTypeId",
                        onDelete: ReferentialAction.Cascade);
                });


            migrationBuilder.CreateIndex(
                name: "IX_FreelancerExperiences_EmploymentTypeId",
                table: "FreelancerExperiences",
                column: "EmploymentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_FreelancerExperiences_UserId",
                table: "FreelancerExperiences",
                column: "UserId");


            migrationBuilder.AddForeignKey(
                name: "FK_FreelancerDetails_Categories_CategoryId",
                table: "FreelancerDetails",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FreelancerDetails_Categories_CategoryId",
                table: "FreelancerDetails");

            migrationBuilder.DropTable(
                name: "FreelancerExperiences");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "FreelancerDetails",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_FreelancerDetails_Categories_CategoryId",
                table: "FreelancerDetails",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "CategoryId");
        }
    }
}
