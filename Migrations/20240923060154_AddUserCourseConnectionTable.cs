using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TGTOAT.Migrations
{
    /// <inheritdoc />
    public partial class AddUserCourseConnectionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserCourseConnection",
                columns: table => new
                {
                    UserCourseConnectionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCourseConnection", x => x.UserCourseConnectionId);
                    table.ForeignKey(
                        name: "FK_UserCourseConnection_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserCourseConnection_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserCourseConnection_CourseId",
                table: "UserCourseConnection",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCourseConnection_UserId",
                table: "UserCourseConnection",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserCourseConnection");
        }
    }
}
