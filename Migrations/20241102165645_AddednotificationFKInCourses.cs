using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TGTOAT.Migrations
{
    /// <inheritdoc />
    public partial class AddednotificationFKInCourses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CoursesCourseId",
                table: "Notifications",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_CoursesCourseId",
                table: "Notifications",
                column: "CoursesCourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Courses_CoursesCourseId",
                table: "Notifications",
                column: "CoursesCourseId",
                principalTable: "Courses",
                principalColumn: "CourseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Courses_CoursesCourseId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_CoursesCourseId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "CoursesCourseId",
                table: "Notifications");
        }
    }
}
