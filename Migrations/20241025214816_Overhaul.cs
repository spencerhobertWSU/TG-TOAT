using System;
using Data;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TGTOAT.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    AddOne = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddTwo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    Zip = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.UserId);
                    table.ForeignKey("FK_Address", x => x.UserId, "User", "UserId", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    DeptId = table.Column<int>(type: "int", nullable: false),
                    DeptName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.DeptId);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    CourseId = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                    DeptId = table.Column<int>(type: "int", nullable: false),
                    CourseNum = table.Column<int>(type: "int", nullable: false),
                    CourseName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CourseDesc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Credits = table.Column<int>(type: "int", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    Campus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Building = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Room = table.Column<int>(type: "int", nullable: true),
                    Days = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    StopTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    Semester = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Picture = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.CourseId);
                    table.ForeignKey("FK_Depts", x => x.DeptId, "Departments", "DeptId", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Assignments",
                columns: table => new
                {
                    AssignId = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    AssignName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AssignDesc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AssignType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaxPoints = table.Column<int>(type: "int", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assignments", x => x.AssignId);
                    table.ForeignKey("FK_AssignCourses", x => x.CourseId, "Courses", "CourseId", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Quizzes",
                columns: table => new
                {
                    QuizId = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    QuizName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuizDesc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumQuestions = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaxPoints = table.Column<int>(type: "int", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Questions = table.Column<string>(type: "nvarchar(max)", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quizzes", x => x.QuizId);
                    table.ForeignKey("FK_QuizCourses", x => x.CourseId, "Courses", "CourseId", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cookies",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Series = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cookies", x => x.UserId);
                    table.ForeignKey("FK_Cookies", x => x.UserId, "User", "UserId", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InstructorConnection",
                columns: table => new
                {
                    InstructorId = table.Column<int>(type: "int", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.ForeignKey("FK_InstructorConnection", x => x.InstructorId, "User", "UserId", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentAssignment",
                columns: table => new
                {
                    AssignId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    Points = table.Column<int>(type: "int", nullable: true),
                    Submitted = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Submission = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.ForeignKey("FK_StudentAssignment", x => x.AssignId, "Assignments", "AssignId", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
                    table.ForeignKey("FK_StudentIdAssign", x => x.StudentId, "User", "UserId", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentQuizzes",
                columns: table => new
                {
                    QuizId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    Points = table.Column<int>(type: "int", nullable: true),
                    Submitted = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Submission = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentQuiz", x => x.QuizId);
                    table.ForeignKey("FK_StudentQuiz", x => x.QuizId, "Quizzes", "QuizId", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
                    table.ForeignKey("FK_StudentIdQuiz", x => x.StudentId, "User", "UserId", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentConnection",
                columns: table => new
                {
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    Grade = table.Column<Decimal>(type: "decimal(5,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.ForeignKey("FK_StudentConnection", x => x.StudentId, "User", "UserId", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserInfo",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PFP = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInfo", x => x.UserId);
                    table.ForeignKey("FK_UserInfo", x => x.UserId, "User", "UserId", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tuition",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    AmountDue = table.Column<Decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tuition", x => x.UserId);
                    table.ForeignKey("FK_Tuition", x => x.UserId, "User", "UserId", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(name: "FK_Notifications_StudentId", column: x => x.StudentId, principalTable: "User", principalColumn: "UserId");
                    table.ForeignKey(name: "FK_Notifications_CourseId", column: x => x.CourseId, principalTable: "Courses", principalColumn: "CourseId");
                });

            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "DeptId", "DeptName" },
                values: new object[,]
                {
                    { 1, "Computer Science" },
                    { 2, "Mathematics" },
                    { 3, "Physics" },
                    { 4, "Biology" },
                    { 5, "Chemistry" }
                });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "UserId", "Email", "Password" },
                values: new object[,]
                {
                    { 1, "ivan@gmail.com ", "99nrQ1U7DPLNkL3SxFNCkA==;W/4hz7jhEXDk6PGC2l3c+iidJiY1uUTSvTU9xytI8qw=" },
                    { 2, "spencer@gmail.com ", "99nrQ1U7DPLNkL3SxFNCkA==;W/4hz7jhEXDk6PGC2l3c+iidJiY1uUTSvTU9xytI8qw=" },
                    { 3, "logan@gmail.com ", "99nrQ1U7DPLNkL3SxFNCkA==;W/4hz7jhEXDk6PGC2l3c+iidJiY1uUTSvTU9xytI8qw=" },
                    { 4, "josh@gmail.com ", "99nrQ1U7DPLNkL3SxFNCkA==;W/4hz7jhEXDk6PGC2l3c+iidJiY1uUTSvTU9xytI8qw=" },
                    { 5, "scott@gmail.com ", "99nrQ1U7DPLNkL3SxFNCkA==;W/4hz7jhEXDk6PGC2l3c+iidJiY1uUTSvTU9xytI8qw=" },
                    { 6, "bob@gmail.com ", "99nrQ1U7DPLNkL3SxFNCkA==;W/4hz7jhEXDk6PGC2l3c+iidJiY1uUTSvTU9xytI8qw=" },
                    { 7, "awstin@gmail.com ", "99nrQ1U7DPLNkL3SxFNCkA==;W/4hz7jhEXDk6PGC2l3c+iidJiY1uUTSvTU9xytI8qw=" },
                    { 8, "alanna@gmail.com ", "99nrQ1U7DPLNkL3SxFNCkA==;W/4hz7jhEXDk6PGC2l3c+iidJiY1uUTSvTU9xytI8qw=" },
                    { 9, "brooks@gmail.com ", "99nrQ1U7DPLNkL3SxFNCkA==;W/4hz7jhEXDk6PGC2l3c+iidJiY1uUTSvTU9xytI8qw=" },
                    { 10, "drew@gmail.com ", "99nrQ1U7DPLNkL3SxFNCkA==;W/4hz7jhEXDk6PGC2l3c+iidJiY1uUTSvTU9xytI8qw=" }
                });

            migrationBuilder.InsertData(
                table: "UserInfo",
                columns: new[] { "UserId", "BirthDate", "FirstName", "LastName", "PFP", "Role" },
                values: new object[,]
                {
                    { 1, new DateOnly(2000, 1, 1), "Ivan", "Garcia", "iVBORw0KGgoAAAANSUhEUgAAAGQAAABkCAYAAABw4pVUAAAAAXNSR0IArs4c6QAABTxJREFUeF7tnQtO4zAQhmtozwGcZOEkC2qQuMXCLZBIRfcklJPQPQePbKYbr7JdYo8T+/e4TCVERRzbmS/z8Gsws4I+Nzc3p29vb9+py03TnBpjTnu/t/R3Y8y2aZrd96Ojo+eHh4d1QY84M9I724NwPpvN6CfoQ3AIkjHm5/Hx8eb+/n4HS+pHLJAeiNtYwus0Z7NYLO6kghEHJAWIfaCd1qzrur6LBTtWPaKAVFVFJukp1sP56pEIRgyQqqp+tAKMZp58MPau30rRFhFAqqoirQh22IFC9xUXASU7ECEwdrDIhK1WqzMfuZTXswKRBMMKuR3XrFer1VVKobvqzgYks8/wyTub+coCBB1N+aT/2XVjzFWOUX4uIM0YISHvyeVP4ECEm6p/mOfwJzmAiNeOnoPfLhaLC+Q0CxTIcrl8bCf6LpGmZ2pbaC2BAmnNVTHa0dcScvB1XW+mwuXcDwNyfX192TTNI6dTAsvAwmAYEImDwADwm7quLwLKjy6KBFKcueqbLdSUCgRI4ebKcrlA+BEIkBKjq09sDsSPKBC+tYf4EQiQwh26RXY4QJbL5Qtt2eG/jPJKoua2UBpSbISFjrQUSIAytuvuyeWVvAF6XjVZfOoQIOrUhQE5hHEIatYXpSE591zxX09HyUMDAt2RGIXAXiWoNXaIhnT7dV9SCApVJyLComeBAKGGSnbsKHMFBVLyjC/KXEGBlGy25vP5GWqjA8xkdQNE3eTgcXpQICVqCVI7oCbLvhglbZRr+wxZlOorDVRDqOGStAQV6mYFQo0XEnFB1tD3XQpcQwoxXXBTZeWSDUg3WJQ4xwVZqh0KtrIC6fwJ7WbMfb4Qum7uinyzAuk5eUqXkesErhgYWcLeobcjcziczWeIceqfgUEfdesSB8B2tnNmprObrP1OIlJrdG2K0Yrs4xDOm5IQjEgQIsJeLpj39/fzpmnI8Y+KxiTmNBEZ9nKA9MuQ1hCcj4+Pb7uIxJHAjHJkURkpOUy4zyrOh3A7fqjlFIgwsgpEgQiTgLDuqIYoEGESENYdqIbQ1EgXqp6QHGzO3T2Z0AH959QHLPfSz/7tQj/nb9fHbeq+wEbq9qE7wQel1LCDuZhwev25DDnRZdPLIhIzJ9GQ2OscFk67Ye0XfZ/P51vfPqmuDzb79Ul7XxCEIUtmEzO349K7FJoTFUhsED7zboVjzUz31o+aXvG1NXCdzGtUMFGAJJwIHCkn7G209zdWtuzJQEra1pMSE2lpjNxak4CgF5RSCjRG3TFmlUcDybzkGkN+KesYveYyCojCYLEcBSUYiJopFgxbKBhKEBCFEQRjl7o8ND0gG4hGU2EwbOnQ6IsN5BDOmo8T6fS7Qs4osoCoqZoOpZ2TY+2m5wKR8P89okglYyWsTdxeIKodURF6tYQDRLUjHhOvlnCAFJ98LJ48p9XEyUrnBFLI0bNpUsLf7TRbTiAa6iah5TRbTiAlJs9PIsKIlfrM1iAQja4iUvi/qkGzNQhE/Uc6IK5kNgokndxdNQ/OAg8CUYeelJQCSSre8MoHIy2XU9cReriguXcoEK6kQOUUCEjQ3GYUCFdSoHIKBCRobjMKhCspUDkFAhI0txkFwpUUqJwCAQma1YxrF4prYCgx2xvrgaUXGguk+P9oIBXMqNle3amYDqcrObNvxVDnsyJzGb1iSP1QLYlM40914zc50N16FiQqFO/xBO++rK9+oDMiDi8MassLxHaIwLy+vj6FHLiP+DDFVhV6RoQNxPoUm9Etw5nwIqD0zsyvqcOhGe1+Az3XJZI28ZenAAAAAElFTkSuQmCC", "Instructor" },
                    { 2, new DateOnly(2000, 1, 1), "Spencer", "Hobert", "iVBORw0KGgoAAAANSUhEUgAAAGQAAABkCAYAAABw4pVUAAAAAXNSR0IArs4c6QAABTxJREFUeF7tnQtO4zAQhmtozwGcZOEkC2qQuMXCLZBIRfcklJPQPQePbKYbr7JdYo8T+/e4TCVERRzbmS/z8Gsws4I+Nzc3p29vb9+py03TnBpjTnu/t/R3Y8y2aZrd96Ojo+eHh4d1QY84M9I724NwPpvN6CfoQ3AIkjHm5/Hx8eb+/n4HS+pHLJAeiNtYwus0Z7NYLO6kghEHJAWIfaCd1qzrur6LBTtWPaKAVFVFJukp1sP56pEIRgyQqqp+tAKMZp58MPau30rRFhFAqqoirQh22IFC9xUXASU7ECEwdrDIhK1WqzMfuZTXswKRBMMKuR3XrFer1VVKobvqzgYks8/wyTub+coCBB1N+aT/2XVjzFWOUX4uIM0YISHvyeVP4ECEm6p/mOfwJzmAiNeOnoPfLhaLC+Q0CxTIcrl8bCf6LpGmZ2pbaC2BAmnNVTHa0dcScvB1XW+mwuXcDwNyfX192TTNI6dTAsvAwmAYEImDwADwm7quLwLKjy6KBFKcueqbLdSUCgRI4ebKcrlA+BEIkBKjq09sDsSPKBC+tYf4EQiQwh26RXY4QJbL5Qtt2eG/jPJKoua2UBpSbISFjrQUSIAytuvuyeWVvAF6XjVZfOoQIOrUhQE5hHEIatYXpSE591zxX09HyUMDAt2RGIXAXiWoNXaIhnT7dV9SCApVJyLComeBAKGGSnbsKHMFBVLyjC/KXEGBlGy25vP5GWqjA8xkdQNE3eTgcXpQICVqCVI7oCbLvhglbZRr+wxZlOorDVRDqOGStAQV6mYFQo0XEnFB1tD3XQpcQwoxXXBTZeWSDUg3WJQ4xwVZqh0KtrIC6fwJ7WbMfb4Qum7uinyzAuk5eUqXkesErhgYWcLeobcjcziczWeIceqfgUEfdesSB8B2tnNmprObrP1OIlJrdG2K0Yrs4xDOm5IQjEgQIsJeLpj39/fzpmnI8Y+KxiTmNBEZ9nKA9MuQ1hCcj4+Pb7uIxJHAjHJkURkpOUy4zyrOh3A7fqjlFIgwsgpEgQiTgLDuqIYoEGESENYdqIbQ1EgXqp6QHGzO3T2Z0AH959QHLPfSz/7tQj/nb9fHbeq+wEbq9qE7wQel1LCDuZhwev25DDnRZdPLIhIzJ9GQ2OscFk67Ye0XfZ/P51vfPqmuDzb79Ul7XxCEIUtmEzO349K7FJoTFUhsED7zboVjzUz31o+aXvG1NXCdzGtUMFGAJJwIHCkn7G209zdWtuzJQEra1pMSE2lpjNxak4CgF5RSCjRG3TFmlUcDybzkGkN+KesYveYyCojCYLEcBSUYiJopFgxbKBhKEBCFEQRjl7o8ND0gG4hGU2EwbOnQ6IsN5BDOmo8T6fS7Qs4osoCoqZoOpZ2TY+2m5wKR8P89okglYyWsTdxeIKodURF6tYQDRLUjHhOvlnCAFJ98LJ48p9XEyUrnBFLI0bNpUsLf7TRbTiAa6iah5TRbTiAlJs9PIsKIlfrM1iAQja4iUvi/qkGzNQhE/Uc6IK5kNgokndxdNQ/OAg8CUYeelJQCSSre8MoHIy2XU9cReriguXcoEK6kQOUUCEjQ3GYUCFdSoHIKBCRobjMKhCspUDkFAhI0txkFwpUUqJwCAQma1YxrF4prYCgx2xvrgaUXGguk+P9oIBXMqNle3amYDqcrObNvxVDnsyJzGb1iSP1QLYlM40914zc50N16FiQqFO/xBO++rK9+oDMiDi8MassLxHaIwLy+vj6FHLiP+DDFVhV6RoQNxPoUm9Etw5nwIqD0zsyvqcOhGe1+Az3XJZI28ZenAAAAAElFTkSuQmCC", "Instructor" },
                    { 3, new DateOnly(2000, 1, 1), "Logan", "McKay", "iVBORw0KGgoAAAANSUhEUgAAAGQAAABkCAYAAABw4pVUAAAAAXNSR0IArs4c6QAABTxJREFUeF7tnQtO4zAQhmtozwGcZOEkC2qQuMXCLZBIRfcklJPQPQePbKYbr7JdYo8T+/e4TCVERRzbmS/z8Gsws4I+Nzc3p29vb9+py03TnBpjTnu/t/R3Y8y2aZrd96Ojo+eHh4d1QY84M9I724NwPpvN6CfoQ3AIkjHm5/Hx8eb+/n4HS+pHLJAeiNtYwus0Z7NYLO6kghEHJAWIfaCd1qzrur6LBTtWPaKAVFVFJukp1sP56pEIRgyQqqp+tAKMZp58MPau30rRFhFAqqoirQh22IFC9xUXASU7ECEwdrDIhK1WqzMfuZTXswKRBMMKuR3XrFer1VVKobvqzgYks8/wyTub+coCBB1N+aT/2XVjzFWOUX4uIM0YISHvyeVP4ECEm6p/mOfwJzmAiNeOnoPfLhaLC+Q0CxTIcrl8bCf6LpGmZ2pbaC2BAmnNVTHa0dcScvB1XW+mwuXcDwNyfX192TTNI6dTAsvAwmAYEImDwADwm7quLwLKjy6KBFKcueqbLdSUCgRI4ebKcrlA+BEIkBKjq09sDsSPKBC+tYf4EQiQwh26RXY4QJbL5Qtt2eG/jPJKoua2UBpSbISFjrQUSIAytuvuyeWVvAF6XjVZfOoQIOrUhQE5hHEIatYXpSE591zxX09HyUMDAt2RGIXAXiWoNXaIhnT7dV9SCApVJyLComeBAKGGSnbsKHMFBVLyjC/KXEGBlGy25vP5GWqjA8xkdQNE3eTgcXpQICVqCVI7oCbLvhglbZRr+wxZlOorDVRDqOGStAQV6mYFQo0XEnFB1tD3XQpcQwoxXXBTZeWSDUg3WJQ4xwVZqh0KtrIC6fwJ7WbMfb4Qum7uinyzAuk5eUqXkesErhgYWcLeobcjcziczWeIceqfgUEfdesSB8B2tnNmprObrP1OIlJrdG2K0Yrs4xDOm5IQjEgQIsJeLpj39/fzpmnI8Y+KxiTmNBEZ9nKA9MuQ1hCcj4+Pb7uIxJHAjHJkURkpOUy4zyrOh3A7fqjlFIgwsgpEgQiTgLDuqIYoEGESENYdqIbQ1EgXqp6QHGzO3T2Z0AH959QHLPfSz/7tQj/nb9fHbeq+wEbq9qE7wQel1LCDuZhwev25DDnRZdPLIhIzJ9GQ2OscFk67Ye0XfZ/P51vfPqmuDzb79Ul7XxCEIUtmEzO349K7FJoTFUhsED7zboVjzUz31o+aXvG1NXCdzGtUMFGAJJwIHCkn7G209zdWtuzJQEra1pMSE2lpjNxak4CgF5RSCjRG3TFmlUcDybzkGkN+KesYveYyCojCYLEcBSUYiJopFgxbKBhKEBCFEQRjl7o8ND0gG4hGU2EwbOnQ6IsN5BDOmo8T6fS7Qs4osoCoqZoOpZ2TY+2m5wKR8P89okglYyWsTdxeIKodURF6tYQDRLUjHhOvlnCAFJ98LJ48p9XEyUrnBFLI0bNpUsLf7TRbTiAa6iah5TRbTiAlJs9PIsKIlfrM1iAQja4iUvi/qkGzNQhE/Uc6IK5kNgokndxdNQ/OAg8CUYeelJQCSSre8MoHIy2XU9cReriguXcoEK6kQOUUCEjQ3GYUCFdSoHIKBCRobjMKhCspUDkFAhI0txkFwpUUqJwCAQma1YxrF4prYCgx2xvrgaUXGguk+P9oIBXMqNle3amYDqcrObNvxVDnsyJzGb1iSP1QLYlM40914zc50N16FiQqFO/xBO++rK9+oDMiDi8MassLxHaIwLy+vj6FHLiP+DDFVhV6RoQNxPoUm9Etw5nwIqD0zsyvqcOhGe1+Az3XJZI28ZenAAAAAElFTkSuQmCC", "Instructor" },
                    { 4, new DateOnly(2000, 1, 1), "Josh", "Morgan", "iVBORw0KGgoAAAANSUhEUgAAAGQAAABkCAYAAABw4pVUAAAAAXNSR0IArs4c6QAABTxJREFUeF7tnQtO4zAQhmtozwGcZOEkC2qQuMXCLZBIRfcklJPQPQePbKYbr7JdYo8T+/e4TCVERRzbmS/z8Gsws4I+Nzc3p29vb9+py03TnBpjTnu/t/R3Y8y2aZrd96Ojo+eHh4d1QY84M9I724NwPpvN6CfoQ3AIkjHm5/Hx8eb+/n4HS+pHLJAeiNtYwus0Z7NYLO6kghEHJAWIfaCd1qzrur6LBTtWPaKAVFVFJukp1sP56pEIRgyQqqp+tAKMZp58MPau30rRFhFAqqoirQh22IFC9xUXASU7ECEwdrDIhK1WqzMfuZTXswKRBMMKuR3XrFer1VVKobvqzgYks8/wyTub+coCBB1N+aT/2XVjzFWOUX4uIM0YISHvyeVP4ECEm6p/mOfwJzmAiNeOnoPfLhaLC+Q0CxTIcrl8bCf6LpGmZ2pbaC2BAmnNVTHa0dcScvB1XW+mwuXcDwNyfX192TTNI6dTAsvAwmAYEImDwADwm7quLwLKjy6KBFKcueqbLdSUCgRI4ebKcrlA+BEIkBKjq09sDsSPKBC+tYf4EQiQwh26RXY4QJbL5Qtt2eG/jPJKoua2UBpSbISFjrQUSIAytuvuyeWVvAF6XjVZfOoQIOrUhQE5hHEIatYXpSE591zxX09HyUMDAt2RGIXAXiWoNXaIhnT7dV9SCApVJyLComeBAKGGSnbsKHMFBVLyjC/KXEGBlGy25vP5GWqjA8xkdQNE3eTgcXpQICVqCVI7oCbLvhglbZRr+wxZlOorDVRDqOGStAQV6mYFQo0XEnFB1tD3XQpcQwoxXXBTZeWSDUg3WJQ4xwVZqh0KtrIC6fwJ7WbMfb4Qum7uinyzAuk5eUqXkesErhgYWcLeobcjcziczWeIceqfgUEfdesSB8B2tnNmprObrP1OIlJrdG2K0Yrs4xDOm5IQjEgQIsJeLpj39/fzpmnI8Y+KxiTmNBEZ9nKA9MuQ1hCcj4+Pb7uIxJHAjHJkURkpOUy4zyrOh3A7fqjlFIgwsgpEgQiTgLDuqIYoEGESENYdqIbQ1EgXqp6QHGzO3T2Z0AH959QHLPfSz/7tQj/nb9fHbeq+wEbq9qE7wQel1LCDuZhwev25DDnRZdPLIhIzJ9GQ2OscFk67Ye0XfZ/P51vfPqmuDzb79Ul7XxCEIUtmEzO349K7FJoTFUhsED7zboVjzUz31o+aXvG1NXCdzGtUMFGAJJwIHCkn7G209zdWtuzJQEra1pMSE2lpjNxak4CgF5RSCjRG3TFmlUcDybzkGkN+KesYveYyCojCYLEcBSUYiJopFgxbKBhKEBCFEQRjl7o8ND0gG4hGU2EwbOnQ6IsN5BDOmo8T6fS7Qs4osoCoqZoOpZ2TY+2m5wKR8P89okglYyWsTdxeIKodURF6tYQDRLUjHhOvlnCAFJ98LJ48p9XEyUrnBFLI0bNpUsLf7TRbTiAa6iah5TRbTiAlJs9PIsKIlfrM1iAQja4iUvi/qkGzNQhE/Uc6IK5kNgokndxdNQ/OAg8CUYeelJQCSSre8MoHIy2XU9cReriguXcoEK6kQOUUCEjQ3GYUCFdSoHIKBCRobjMKhCspUDkFAhI0txkFwpUUqJwCAQma1YxrF4prYCgx2xvrgaUXGguk+P9oIBXMqNle3amYDqcrObNvxVDnsyJzGb1iSP1QLYlM40914zc50N16FiQqFO/xBO++rK9+oDMiDi8MassLxHaIwLy+vj6FHLiP+DDFVhV6RoQNxPoUm9Etw5nwIqD0zsyvqcOhGe1+Az3XJZI28ZenAAAAAElFTkSuQmCC", "Instructor" },
                    { 5, new DateOnly(2000, 1, 1), "Scott", "Van Horn", "iVBORw0KGgoAAAANSUhEUgAAAGQAAABkCAYAAABw4pVUAAAAAXNSR0IArs4c6QAABTxJREFUeF7tnQtO4zAQhmtozwGcZOEkC2qQuMXCLZBIRfcklJPQPQePbKYbr7JdYo8T+/e4TCVERRzbmS/z8Gsws4I+Nzc3p29vb9+py03TnBpjTnu/t/R3Y8y2aZrd96Ojo+eHh4d1QY84M9I724NwPpvN6CfoQ3AIkjHm5/Hx8eb+/n4HS+pHLJAeiNtYwus0Z7NYLO6kghEHJAWIfaCd1qzrur6LBTtWPaKAVFVFJukp1sP56pEIRgyQqqp+tAKMZp58MPau30rRFhFAqqoirQh22IFC9xUXASU7ECEwdrDIhK1WqzMfuZTXswKRBMMKuR3XrFer1VVKobvqzgYks8/wyTub+coCBB1N+aT/2XVjzFWOUX4uIM0YISHvyeVP4ECEm6p/mOfwJzmAiNeOnoPfLhaLC+Q0CxTIcrl8bCf6LpGmZ2pbaC2BAmnNVTHa0dcScvB1XW+mwuXcDwNyfX192TTNI6dTAsvAwmAYEImDwADwm7quLwLKjy6KBFKcueqbLdSUCgRI4ebKcrlA+BEIkBKjq09sDsSPKBC+tYf4EQiQwh26RXY4QJbL5Qtt2eG/jPJKoua2UBpSbISFjrQUSIAytuvuyeWVvAF6XjVZfOoQIOrUhQE5hHEIatYXpSE591zxX09HyUMDAt2RGIXAXiWoNXaIhnT7dV9SCApVJyLComeBAKGGSnbsKHMFBVLyjC/KXEGBlGy25vP5GWqjA8xkdQNE3eTgcXpQICVqCVI7oCbLvhglbZRr+wxZlOorDVRDqOGStAQV6mYFQo0XEnFB1tD3XQpcQwoxXXBTZeWSDUg3WJQ4xwVZqh0KtrIC6fwJ7WbMfb4Qum7uinyzAuk5eUqXkesErhgYWcLeobcjcziczWeIceqfgUEfdesSB8B2tnNmprObrP1OIlJrdG2K0Yrs4xDOm5IQjEgQIsJeLpj39/fzpmnI8Y+KxiTmNBEZ9nKA9MuQ1hCcj4+Pb7uIxJHAjHJkURkpOUy4zyrOh3A7fqjlFIgwsgpEgQiTgLDuqIYoEGESENYdqIbQ1EgXqp6QHGzO3T2Z0AH959QHLPfSz/7tQj/nb9fHbeq+wEbq9qE7wQel1LCDuZhwev25DDnRZdPLIhIzJ9GQ2OscFk67Ye0XfZ/P51vfPqmuDzb79Ul7XxCEIUtmEzO349K7FJoTFUhsED7zboVjzUz31o+aXvG1NXCdzGtUMFGAJJwIHCkn7G209zdWtuzJQEra1pMSE2lpjNxak4CgF5RSCjRG3TFmlUcDybzkGkN+KesYveYyCojCYLEcBSUYiJopFgxbKBhKEBCFEQRjl7o8ND0gG4hGU2EwbOnQ6IsN5BDOmo8T6fS7Qs4osoCoqZoOpZ2TY+2m5wKR8P89okglYyWsTdxeIKodURF6tYQDRLUjHhOvlnCAFJ98LJ48p9XEyUrnBFLI0bNpUsLf7TRbTiAa6iah5TRbTiAlJs9PIsKIlfrM1iAQja4iUvi/qkGzNQhE/Uc6IK5kNgokndxdNQ/OAg8CUYeelJQCSSre8MoHIy2XU9cReriguXcoEK6kQOUUCEjQ3GYUCFdSoHIKBCRobjMKhCspUDkFAhI0txkFwpUUqJwCAQma1YxrF4prYCgx2xvrgaUXGguk+P9oIBXMqNle3amYDqcrObNvxVDnsyJzGb1iSP1QLYlM40914zc50N16FiQqFO/xBO++rK9+oDMiDi8MassLxHaIwLy+vj6FHLiP+DDFVhV6RoQNxPoUm9Etw5nwIqD0zsyvqcOhGe1+Az3XJZI28ZenAAAAAElFTkSuQmCC", "Instructor" },
                    { 6, new DateOnly(2000, 1, 1), "Bob", "Johnson", "iVBORw0KGgoAAAANSUhEUgAAAGQAAABkCAYAAABw4pVUAAAAAXNSR0IArs4c6QAABTxJREFUeF7tnQtO4zAQhmtozwGcZOEkC2qQuMXCLZBIRfcklJPQPQePbKYbr7JdYo8T+/e4TCVERRzbmS/z8Gsws4I+Nzc3p29vb9+py03TnBpjTnu/t/R3Y8y2aZrd96Ojo+eHh4d1QY84M9I724NwPpvN6CfoQ3AIkjHm5/Hx8eb+/n4HS+pHLJAeiNtYwus0Z7NYLO6kghEHJAWIfaCd1qzrur6LBTtWPaKAVFVFJukp1sP56pEIRgyQqqp+tAKMZp58MPau30rRFhFAqqoirQh22IFC9xUXASU7ECEwdrDIhK1WqzMfuZTXswKRBMMKuR3XrFer1VVKobvqzgYks8/wyTub+coCBB1N+aT/2XVjzFWOUX4uIM0YISHvyeVP4ECEm6p/mOfwJzmAiNeOnoPfLhaLC+Q0CxTIcrl8bCf6LpGmZ2pbaC2BAmnNVTHa0dcScvB1XW+mwuXcDwNyfX192TTNI6dTAsvAwmAYEImDwADwm7quLwLKjy6KBFKcueqbLdSUCgRI4ebKcrlA+BEIkBKjq09sDsSPKBC+tYf4EQiQwh26RXY4QJbL5Qtt2eG/jPJKoua2UBpSbISFjrQUSIAytuvuyeWVvAF6XjVZfOoQIOrUhQE5hHEIatYXpSE591zxX09HyUMDAt2RGIXAXiWoNXaIhnT7dV9SCApVJyLComeBAKGGSnbsKHMFBVLyjC/KXEGBlGy25vP5GWqjA8xkdQNE3eTgcXpQICVqCVI7oCbLvhglbZRr+wxZlOorDVRDqOGStAQV6mYFQo0XEnFB1tD3XQpcQwoxXXBTZeWSDUg3WJQ4xwVZqh0KtrIC6fwJ7WbMfb4Qum7uinyzAuk5eUqXkesErhgYWcLeobcjcziczWeIceqfgUEfdesSB8B2tnNmprObrP1OIlJrdG2K0Yrs4xDOm5IQjEgQIsJeLpj39/fzpmnI8Y+KxiTmNBEZ9nKA9MuQ1hCcj4+Pb7uIxJHAjHJkURkpOUy4zyrOh3A7fqjlFIgwsgpEgQiTgLDuqIYoEGESENYdqIbQ1EgXqp6QHGzO3T2Z0AH959QHLPfSz/7tQj/nb9fHbeq+wEbq9qE7wQel1LCDuZhwev25DDnRZdPLIhIzJ9GQ2OscFk67Ye0XfZ/P51vfPqmuDzb79Ul7XxCEIUtmEzO349K7FJoTFUhsED7zboVjzUz31o+aXvG1NXCdzGtUMFGAJJwIHCkn7G209zdWtuzJQEra1pMSE2lpjNxak4CgF5RSCjRG3TFmlUcDybzkGkN+KesYveYyCojCYLEcBSUYiJopFgxbKBhKEBCFEQRjl7o8ND0gG4hGU2EwbOnQ6IsN5BDOmo8T6fS7Qs4osoCoqZoOpZ2TY+2m5wKR8P89okglYyWsTdxeIKodURF6tYQDRLUjHhOvlnCAFJ98LJ48p9XEyUrnBFLI0bNpUsLf7TRbTiAa6iah5TRbTiAlJs9PIsKIlfrM1iAQja4iUvi/qkGzNQhE/Uc6IK5kNgokndxdNQ/OAg8CUYeelJQCSSre8MoHIy2XU9cReriguXcoEK6kQOUUCEjQ3GYUCFdSoHIKBCRobjMKhCspUDkFAhI0txkFwpUUqJwCAQma1YxrF4prYCgx2xvrgaUXGguk+P9oIBXMqNle3amYDqcrObNvxVDnsyJzGb1iSP1QLYlM40914zc50N16FiQqFO/xBO++rK9+oDMiDi8MassLxHaIwLy+vj6FHLiP+DDFVhV6RoQNxPoUm9Etw5nwIqD0zsyvqcOhGe1+Az3XJZI28ZenAAAAAElFTkSuQmCC", "Student" },
                    { 7, new DateOnly(2000, 1, 1), "Awstin", "Prashat", "iVBORw0KGgoAAAANSUhEUgAAAGQAAABkCAYAAABw4pVUAAAAAXNSR0IArs4c6QAABTxJREFUeF7tnQtO4zAQhmtozwGcZOEkC2qQuMXCLZBIRfcklJPQPQePbKYbr7JdYo8T+/e4TCVERRzbmS/z8Gsws4I+Nzc3p29vb9+py03TnBpjTnu/t/R3Y8y2aZrd96Ojo+eHh4d1QY84M9I724NwPpvN6CfoQ3AIkjHm5/Hx8eb+/n4HS+pHLJAeiNtYwus0Z7NYLO6kghEHJAWIfaCd1qzrur6LBTtWPaKAVFVFJukp1sP56pEIRgyQqqp+tAKMZp58MPau30rRFhFAqqoirQh22IFC9xUXASU7ECEwdrDIhK1WqzMfuZTXswKRBMMKuR3XrFer1VVKobvqzgYks8/wyTub+coCBB1N+aT/2XVjzFWOUX4uIM0YISHvyeVP4ECEm6p/mOfwJzmAiNeOnoPfLhaLC+Q0CxTIcrl8bCf6LpGmZ2pbaC2BAmnNVTHa0dcScvB1XW+mwuXcDwNyfX192TTNI6dTAsvAwmAYEImDwADwm7quLwLKjy6KBFKcueqbLdSUCgRI4ebKcrlA+BEIkBKjq09sDsSPKBC+tYf4EQiQwh26RXY4QJbL5Qtt2eG/jPJKoua2UBpSbISFjrQUSIAytuvuyeWVvAF6XjVZfOoQIOrUhQE5hHEIatYXpSE591zxX09HyUMDAt2RGIXAXiWoNXaIhnT7dV9SCApVJyLComeBAKGGSnbsKHMFBVLyjC/KXEGBlGy25vP5GWqjA8xkdQNE3eTgcXpQICVqCVI7oCbLvhglbZRr+wxZlOorDVRDqOGStAQV6mYFQo0XEnFB1tD3XQpcQwoxXXBTZeWSDUg3WJQ4xwVZqh0KtrIC6fwJ7WbMfb4Qum7uinyzAuk5eUqXkesErhgYWcLeobcjcziczWeIceqfgUEfdesSB8B2tnNmprObrP1OIlJrdG2K0Yrs4xDOm5IQjEgQIsJeLpj39/fzpmnI8Y+KxiTmNBEZ9nKA9MuQ1hCcj4+Pb7uIxJHAjHJkURkpOUy4zyrOh3A7fqjlFIgwsgpEgQiTgLDuqIYoEGESENYdqIbQ1EgXqp6QHGzO3T2Z0AH959QHLPfSz/7tQj/nb9fHbeq+wEbq9qE7wQel1LCDuZhwev25DDnRZdPLIhIzJ9GQ2OscFk67Ye0XfZ/P51vfPqmuDzb79Ul7XxCEIUtmEzO349K7FJoTFUhsED7zboVjzUz31o+aXvG1NXCdzGtUMFGAJJwIHCkn7G209zdWtuzJQEra1pMSE2lpjNxak4CgF5RSCjRG3TFmlUcDybzkGkN+KesYveYyCojCYLEcBSUYiJopFgxbKBhKEBCFEQRjl7o8ND0gG4hGU2EwbOnQ6IsN5BDOmo8T6fS7Qs4osoCoqZoOpZ2TY+2m5wKR8P89okglYyWsTdxeIKodURF6tYQDRLUjHhOvlnCAFJ98LJ48p9XEyUrnBFLI0bNpUsLf7TRbTiAa6iah5TRbTiAlJs9PIsKIlfrM1iAQja4iUvi/qkGzNQhE/Uc6IK5kNgokndxdNQ/OAg8CUYeelJQCSSre8MoHIy2XU9cReriguXcoEK6kQOUUCEjQ3GYUCFdSoHIKBCRobjMKhCspUDkFAhI0txkFwpUUqJwCAQma1YxrF4prYCgx2xvrgaUXGguk+P9oIBXMqNle3amYDqcrObNvxVDnsyJzGb1iSP1QLYlM40914zc50N16FiQqFO/xBO++rK9+oDMiDi8MassLxHaIwLy+vj6FHLiP+DDFVhV6RoQNxPoUm9Etw5nwIqD0zsyvqcOhGe1+Az3XJZI28ZenAAAAAElFTkSuQmCC", "Student" },
                    { 8, new DateOnly(2000, 1, 1), "Alanna", "Hardin", "iVBORw0KGgoAAAANSUhEUgAAAGQAAABkCAYAAABw4pVUAAAAAXNSR0IArs4c6QAABTxJREFUeF7tnQtO4zAQhmtozwGcZOEkC2qQuMXCLZBIRfcklJPQPQePbKYbr7JdYo8T+/e4TCVERRzbmS/z8Gsws4I+Nzc3p29vb9+py03TnBpjTnu/t/R3Y8y2aZrd96Ojo+eHh4d1QY84M9I724NwPpvN6CfoQ3AIkjHm5/Hx8eb+/n4HS+pHLJAeiNtYwus0Z7NYLO6kghEHJAWIfaCd1qzrur6LBTtWPaKAVFVFJukp1sP56pEIRgyQqqp+tAKMZp58MPau30rRFhFAqqoirQh22IFC9xUXASU7ECEwdrDIhK1WqzMfuZTXswKRBMMKuR3XrFer1VVKobvqzgYks8/wyTub+coCBB1N+aT/2XVjzFWOUX4uIM0YISHvyeVP4ECEm6p/mOfwJzmAiNeOnoPfLhaLC+Q0CxTIcrl8bCf6LpGmZ2pbaC2BAmnNVTHa0dcScvB1XW+mwuXcDwNyfX192TTNI6dTAsvAwmAYEImDwADwm7quLwLKjy6KBFKcueqbLdSUCgRI4ebKcrlA+BEIkBKjq09sDsSPKBC+tYf4EQiQwh26RXY4QJbL5Qtt2eG/jPJKoua2UBpSbISFjrQUSIAytuvuyeWVvAF6XjVZfOoQIOrUhQE5hHEIatYXpSE591zxX09HyUMDAt2RGIXAXiWoNXaIhnT7dV9SCApVJyLComeBAKGGSnbsKHMFBVLyjC/KXEGBlGy25vP5GWqjA8xkdQNE3eTgcXpQICVqCVI7oCbLvhglbZRr+wxZlOorDVRDqOGStAQV6mYFQo0XEnFB1tD3XQpcQwoxXXBTZeWSDUg3WJQ4xwVZqh0KtrIC6fwJ7WbMfb4Qum7uinyzAuk5eUqXkesErhgYWcLeobcjcziczWeIceqfgUEfdesSB8B2tnNmprObrP1OIlJrdG2K0Yrs4xDOm5IQjEgQIsJeLpj39/fzpmnI8Y+KxiTmNBEZ9nKA9MuQ1hCcj4+Pb7uIxJHAjHJkURkpOUy4zyrOh3A7fqjlFIgwsgpEgQiTgLDuqIYoEGESENYdqIbQ1EgXqp6QHGzO3T2Z0AH959QHLPfSz/7tQj/nb9fHbeq+wEbq9qE7wQel1LCDuZhwev25DDnRZdPLIhIzJ9GQ2OscFk67Ye0XfZ/P51vfPqmuDzb79Ul7XxCEIUtmEzO349K7FJoTFUhsED7zboVjzUz31o+aXvG1NXCdzGtUMFGAJJwIHCkn7G209zdWtuzJQEra1pMSE2lpjNxak4CgF5RSCjRG3TFmlUcDybzkGkN+KesYveYyCojCYLEcBSUYiJopFgxbKBhKEBCFEQRjl7o8ND0gG4hGU2EwbOnQ6IsN5BDOmo8T6fS7Qs4osoCoqZoOpZ2TY+2m5wKR8P89okglYyWsTdxeIKodURF6tYQDRLUjHhOvlnCAFJ98LJ48p9XEyUrnBFLI0bNpUsLf7TRbTiAa6iah5TRbTiAlJs9PIsKIlfrM1iAQja4iUvi/qkGzNQhE/Uc6IK5kNgokndxdNQ/OAg8CUYeelJQCSSre8MoHIy2XU9cReriguXcoEK6kQOUUCEjQ3GYUCFdSoHIKBCRobjMKhCspUDkFAhI0txkFwpUUqJwCAQma1YxrF4prYCgx2xvrgaUXGguk+P9oIBXMqNle3amYDqcrObNvxVDnsyJzGb1iSP1QLYlM40914zc50N16FiQqFO/xBO++rK9+oDMiDi8MassLxHaIwLy+vj6FHLiP+DDFVhV6RoQNxPoUm9Etw5nwIqD0zsyvqcOhGe1+Az3XJZI28ZenAAAAAElFTkSuQmCC", "Student" },
                    { 9, new DateOnly(2000, 1, 1), "Brooks", "Gray", "iVBORw0KGgoAAAANSUhEUgAAAGQAAABkCAYAAABw4pVUAAAAAXNSR0IArs4c6QAABTxJREFUeF7tnQtO4zAQhmtozwGcZOEkC2qQuMXCLZBIRfcklJPQPQePbKYbr7JdYo8T+/e4TCVERRzbmS/z8Gsws4I+Nzc3p29vb9+py03TnBpjTnu/t/R3Y8y2aZrd96Ojo+eHh4d1QY84M9I724NwPpvN6CfoQ3AIkjHm5/Hx8eb+/n4HS+pHLJAeiNtYwus0Z7NYLO6kghEHJAWIfaCd1qzrur6LBTtWPaKAVFVFJukp1sP56pEIRgyQqqp+tAKMZp58MPau30rRFhFAqqoirQh22IFC9xUXASU7ECEwdrDIhK1WqzMfuZTXswKRBMMKuR3XrFer1VVKobvqzgYks8/wyTub+coCBB1N+aT/2XVjzFWOUX4uIM0YISHvyeVP4ECEm6p/mOfwJzmAiNeOnoPfLhaLC+Q0CxTIcrl8bCf6LpGmZ2pbaC2BAmnNVTHa0dcScvB1XW+mwuXcDwNyfX192TTNI6dTAsvAwmAYEImDwADwm7quLwLKjy6KBFKcueqbLdSUCgRI4ebKcrlA+BEIkBKjq09sDsSPKBC+tYf4EQiQwh26RXY4QJbL5Qtt2eG/jPJKoua2UBpSbISFjrQUSIAytuvuyeWVvAF6XjVZfOoQIOrUhQE5hHEIatYXpSE591zxX09HyUMDAt2RGIXAXiWoNXaIhnT7dV9SCApVJyLComeBAKGGSnbsKHMFBVLyjC/KXEGBlGy25vP5GWqjA8xkdQNE3eTgcXpQICVqCVI7oCbLvhglbZRr+wxZlOorDVRDqOGStAQV6mYFQo0XEnFB1tD3XQpcQwoxXXBTZeWSDUg3WJQ4xwVZqh0KtrIC6fwJ7WbMfb4Qum7uinyzAuk5eUqXkesErhgYWcLeobcjcziczWeIceqfgUEfdesSB8B2tnNmprObrP1OIlJrdG2K0Yrs4xDOm5IQjEgQIsJeLpj39/fzpmnI8Y+KxiTmNBEZ9nKA9MuQ1hCcj4+Pb7uIxJHAjHJkURkpOUy4zyrOh3A7fqjlFIgwsgpEgQiTgLDuqIYoEGESENYdqIbQ1EgXqp6QHGzO3T2Z0AH959QHLPfSz/7tQj/nb9fHbeq+wEbq9qE7wQel1LCDuZhwev25DDnRZdPLIhIzJ9GQ2OscFk67Ye0XfZ/P51vfPqmuDzb79Ul7XxCEIUtmEzO349K7FJoTFUhsED7zboVjzUz31o+aXvG1NXCdzGtUMFGAJJwIHCkn7G209zdWtuzJQEra1pMSE2lpjNxak4CgF5RSCjRG3TFmlUcDybzkGkN+KesYveYyCojCYLEcBSUYiJopFgxbKBhKEBCFEQRjl7o8ND0gG4hGU2EwbOnQ6IsN5BDOmo8T6fS7Qs4osoCoqZoOpZ2TY+2m5wKR8P89okglYyWsTdxeIKodURF6tYQDRLUjHhOvlnCAFJ98LJ48p9XEyUrnBFLI0bNpUsLf7TRbTiAa6iah5TRbTiAlJs9PIsKIlfrM1iAQja4iUvi/qkGzNQhE/Uc6IK5kNgokndxdNQ/OAg8CUYeelJQCSSre8MoHIy2XU9cReriguXcoEK6kQOUUCEjQ3GYUCFdSoHIKBCRobjMKhCspUDkFAhI0txkFwpUUqJwCAQma1YxrF4prYCgx2xvrgaUXGguk+P9oIBXMqNle3amYDqcrObNvxVDnsyJzGb1iSP1QLYlM40914zc50N16FiQqFO/xBO++rK9+oDMiDi8MassLxHaIwLy+vj6FHLiP+DDFVhV6RoQNxPoUm9Etw5nwIqD0zsyvqcOhGe1+Az3XJZI28ZenAAAAAElFTkSuQmCC", "Student" },
                    { 10, new DateOnly(2000, 1, 1), "Drew", "Barnes", "iVBORw0KGgoAAAANSUhEUgAAAGQAAABkCAYAAABw4pVUAAAAAXNSR0IArs4c6QAABTxJREFUeF7tnQtO4zAQhmtozwGcZOEkC2qQuMXCLZBIRfcklJPQPQePbKYbr7JdYo8T+/e4TCVERRzbmS/z8Gsws4I+Nzc3p29vb9+py03TnBpjTnu/t/R3Y8y2aZrd96Ojo+eHh4d1QY84M9I724NwPpvN6CfoQ3AIkjHm5/Hx8eb+/n4HS+pHLJAeiNtYwus0Z7NYLO6kghEHJAWIfaCd1qzrur6LBTtWPaKAVFVFJukp1sP56pEIRgyQqqp+tAKMZp58MPau30rRFhFAqqoirQh22IFC9xUXASU7ECEwdrDIhK1WqzMfuZTXswKRBMMKuR3XrFer1VVKobvqzgYks8/wyTub+coCBB1N+aT/2XVjzFWOUX4uIM0YISHvyeVP4ECEm6p/mOfwJzmAiNeOnoPfLhaLC+Q0CxTIcrl8bCf6LpGmZ2pbaC2BAmnNVTHa0dcScvB1XW+mwuXcDwNyfX192TTNI6dTAsvAwmAYEImDwADwm7quLwLKjy6KBFKcueqbLdSUCgRI4ebKcrlA+BEIkBKjq09sDsSPKBC+tYf4EQiQwh26RXY4QJbL5Qtt2eG/jPJKoua2UBpSbISFjrQUSIAytuvuyeWVvAF6XjVZfOoQIOrUhQE5hHEIatYXpSE591zxX09HyUMDAt2RGIXAXiWoNXaIhnT7dV9SCApVJyLComeBAKGGSnbsKHMFBVLyjC/KXEGBlGy25vP5GWqjA8xkdQNE3eTgcXpQICVqCVI7oCbLvhglbZRr+wxZlOorDVRDqOGStAQV6mYFQo0XEnFB1tD3XQpcQwoxXXBTZeWSDUg3WJQ4xwVZqh0KtrIC6fwJ7WbMfb4Qum7uinyzAuk5eUqXkesErhgYWcLeobcjcziczWeIceqfgUEfdesSB8B2tnNmprObrP1OIlJrdG2K0Yrs4xDOm5IQjEgQIsJeLpj39/fzpmnI8Y+KxiTmNBEZ9nKA9MuQ1hCcj4+Pb7uIxJHAjHJkURkpOUy4zyrOh3A7fqjlFIgwsgpEgQiTgLDuqIYoEGESENYdqIbQ1EgXqp6QHGzO3T2Z0AH959QHLPfSz/7tQj/nb9fHbeq+wEbq9qE7wQel1LCDuZhwev25DDnRZdPLIhIzJ9GQ2OscFk67Ye0XfZ/P51vfPqmuDzb79Ul7XxCEIUtmEzO349K7FJoTFUhsED7zboVjzUz31o+aXvG1NXCdzGtUMFGAJJwIHCkn7G209zdWtuzJQEra1pMSE2lpjNxak4CgF5RSCjRG3TFmlUcDybzkGkN+KesYveYyCojCYLEcBSUYiJopFgxbKBhKEBCFEQRjl7o8ND0gG4hGU2EwbOnQ6IsN5BDOmo8T6fS7Qs4osoCoqZoOpZ2TY+2m5wKR8P89okglYyWsTdxeIKodURF6tYQDRLUjHhOvlnCAFJ98LJ48p9XEyUrnBFLI0bNpUsLf7TRbTiAa6iah5TRbTiAlJs9PIsKIlfrM1iAQja4iUvi/qkGzNQhE/Uc6IK5kNgokndxdNQ/OAg8CUYeelJQCSSre8MoHIy2XU9cReriguXcoEK6kQOUUCEjQ3GYUCFdSoHIKBCRobjMKhCspUDkFAhI0txkFwpUUqJwCAQma1YxrF4prYCgx2xvrgaUXGguk+P9oIBXMqNle3amYDqcrObNvxVDnsyJzGb1iSP1QLYlM40914zc50N16FiQqFO/xBO++rK9+oDMiDi8MassLxHaIwLy+vj6FHLiP+DDFVhV6RoQNxPoUm9Etw5nwIqD0zsyvqcOhGe1+Az3XJZI28ZenAAAAAElFTkSuQmCC", "Student" }
                });

            migrationBuilder.InsertData(
                table: "Address",
                columns: new[] { "UserId", "AddOne", "AddTwo", "City", "State", "Zip" },
                values: new object[,]
                {
                    { 1, null, null, null, null, null },
                    { 2, null, null, null, null, null },
                    { 3, null, null, null, null, null },
                    { 4, null, null, null, null, null },
                    { 5, null, null, null, null, null },
                    { 6, null, null, null, null, null },
                    { 7, null, null, null, null, null },
                    { 8, null, null, null, null, null },
                    { 9, null, null, null, null, null },
                    { 10, null, null, null, null, null }
                });

            migrationBuilder.InsertData(
                table: "Tuition",
                columns: new[] { "UserId", "AmountDue" },
                values: new object[,]
                {
                    { 6, 0 },
                    { 7, 0 },
                    { 8, 0 },
                    { 9, 0 },
                    { 10, 0 }
                });

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.DropTable(
                name: "Assignments");

            migrationBuilder.DropTable(
                name: "Cookies");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "InstructorConnection");

            migrationBuilder.DropTable(
                name: "StudentAssignment");

            migrationBuilder.DropTable(
                name: "StudentConnection");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "UserInfo");
        }
    }
}
