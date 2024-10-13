using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TGTOAT.Migrations
{
    /// <inheritdoc />
    public partial class CreateStudentAssignments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Submission",
                table: "Assignments");

            migrationBuilder.CreateTable(
                name: "StudentAssignment",
                columns: table => new
                {
                    AssignmentGradeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssignmentId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    studentCourseConnectionStudentCourseId = table.Column<int>(type: "int", nullable: true),
                    Grade = table.Column<int>(type: "int", nullable: false),
                    SubmissionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TextSubmission = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileSubmission = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentAssignment", x => x.AssignmentGradeId);
                    table.ForeignKey(
                        name: "FK_StudentAssignment_Assignments_AssignmentId",
                        column: x => x.AssignmentId,
                        principalTable: "Assignments",
                        principalColumn: "AssignmentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentAssignment_StudentCourseConnection_studentCourseConnectionStudentCourseId",
                        column: x => x.studentCourseConnectionStudentCourseId,
                        principalTable: "StudentCourseConnection",
                        principalColumn: "StudentCourseId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentAssignment_AssignmentId",
                table: "StudentAssignment",
                column: "AssignmentId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_StudentAssignment_studentCourseConnectionStudentCourseId",
            //    table: "StudentAssignment",
            //    column: "studentCourseConnectionStudentCourseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentAssignment");

            migrationBuilder.AddColumn<string>(
                name: "Submission",
                table: "Assignments",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
