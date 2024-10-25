using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TGTOAT.Migrations
{
    /// <inheritdoc />
    public partial class AzureSqlServerMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    DepartmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.DepartmentId);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    CourseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    CourseNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CourseName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CourseDescription = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    NumberOfCredits = table.Column<int>(type: "int", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    Campus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Building = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RoomNumber = table.Column<int>(type: "int", nullable: false),
                    DaysOfTheWeek = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    Semester = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.CourseId);
                    table.ForeignKey(
                        name: "FK_Courses_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "DepartmentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserRole = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProfileImageBase64 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AmountDue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CoursesCourseId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Courses_CoursesCourseId",
                        column: x => x.CoursesCourseId,
                        principalTable: "Courses",
                        principalColumn: "CourseId");
                });

            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    AddressLineOne = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    AddressLineTwo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ZipCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Address_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InstructorCourseConnection",
                columns: table => new
                {
                    InstructorCourseConnectionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InstructorID = table.Column<int>(type: "int", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstructorCourseConnection", x => x.InstructorCourseConnectionId);
                    table.ForeignKey(
                        name: "FK_InstructorCourseConnection_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InstructorCourseConnection_User_InstructorID",
                        column: x => x.InstructorID,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentCourseConnection",
                columns: table => new
                {
                    StudentCourseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentID = table.Column<int>(type: "int", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentCourseConnection", x => x.StudentCourseId);
                    table.ForeignKey(
                        name: "FK_StudentCourseConnection_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentCourseConnection_User_StudentID",
                        column: x => x.StudentID,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Assignments",
                columns: table => new
                {
                    AssignmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InstructorCourseId = table.Column<int>(type: "int", nullable: false),
                    AssignmentDescription = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    AssignmentName = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    DueDateAndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AssignmentPoints = table.Column<int>(type: "int", nullable: false),
                    AssignmentType = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assignments", x => x.AssignmentId);
                    table.ForeignKey(
                        name: "FK_Assignments_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Assignments_InstructorCourseConnection_InstructorCourseId",
                        column: x => x.InstructorCourseId,
                        principalTable: "InstructorCourseConnection",
                        principalColumn: "InstructorCourseConnectionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StudentAssignment",
                columns: table => new
                {
                    AssignmentGradeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssignmentId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    studentCourseConnectionStudentCourseId = table.Column<int>(type: "int", nullable: true),
                    Grade = table.Column<int>(type: "int", nullable: true),
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

            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "DepartmentId", "DepartmentName" },
                values: new object[,]
                {
                    { 1, "Computer Science" },
                    { 2, "Mathematics" },
                    { 3, "Physics" },
                    { 4, "Biology" },
                    { 5, "Chemistry" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_CourseId",
                table: "Assignments",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_InstructorCourseId",
                table: "Assignments",
                column: "InstructorCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_DepartmentId",
                table: "Courses",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_InstructorCourseConnection_CourseId",
                table: "InstructorCourseConnection",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_InstructorCourseConnection_InstructorID",
                table: "InstructorCourseConnection",
                column: "InstructorID");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAssignment_AssignmentId",
                table: "StudentAssignment",
                column: "AssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAssignment_studentCourseConnectionStudentCourseId",
                table: "StudentAssignment",
                column: "studentCourseConnectionStudentCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentCourseConnection_CourseId",
                table: "StudentCourseConnection",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentCourseConnection_StudentID",
                table: "StudentCourseConnection",
                column: "StudentID");

            migrationBuilder.CreateIndex(
                name: "IX_User_CoursesCourseId",
                table: "User",
                column: "CoursesCourseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.DropTable(
                name: "StudentAssignment");

            migrationBuilder.DropTable(
                name: "Assignments");

            migrationBuilder.DropTable(
                name: "StudentCourseConnection");

            migrationBuilder.DropTable(
                name: "InstructorCourseConnection");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "Departments");
        }
    }
}
