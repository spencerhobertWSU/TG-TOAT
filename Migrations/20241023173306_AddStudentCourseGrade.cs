using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TGTOAT.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentCourseGrade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Grade",
                table: "StudentCourseConnection",
                type: "decimal(5,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Grade",
                table: "StudentCourseConnection");
        }
    }
}
