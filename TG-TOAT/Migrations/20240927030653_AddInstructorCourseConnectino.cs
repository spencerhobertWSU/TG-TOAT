using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TGTOAT.Migrations
{
    /// <inheritdoc />
    public partial class AddInstructorCourseConnectino : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserCourseConnection_Courses_CourseId",
                table: "UserCourseConnection");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCourseConnection_User_UserId",
                table: "UserCourseConnection");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserCourseConnection",
                table: "UserCourseConnection");

            migrationBuilder.RenameTable(
                name: "UserCourseConnection",
                newName: "InstructorCourseConnection");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "InstructorCourseConnection",
                newName: "InstructorID");

            migrationBuilder.RenameColumn(
                name: "UserCourseConnectionId",
                table: "InstructorCourseConnection",
                newName: "InstructorCourseConnectionId");

            migrationBuilder.RenameIndex(
                name: "IX_UserCourseConnection_UserId",
                table: "InstructorCourseConnection",
                newName: "IX_InstructorCourseConnection_InstructorID");

            migrationBuilder.RenameIndex(
                name: "IX_UserCourseConnection_CourseId",
                table: "InstructorCourseConnection",
                newName: "IX_InstructorCourseConnection_CourseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InstructorCourseConnection",
                table: "InstructorCourseConnection",
                column: "InstructorCourseConnectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_InstructorCourseConnection_Courses_CourseId",
                table: "InstructorCourseConnection",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InstructorCourseConnection_User_InstructorID",
                table: "InstructorCourseConnection",
                column: "InstructorID",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InstructorCourseConnection_Courses_CourseId",
                table: "InstructorCourseConnection");

            migrationBuilder.DropForeignKey(
                name: "FK_InstructorCourseConnection_User_InstructorID",
                table: "InstructorCourseConnection");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InstructorCourseConnection",
                table: "InstructorCourseConnection");

            migrationBuilder.RenameTable(
                name: "InstructorCourseConnection",
                newName: "UserCourseConnection");

            migrationBuilder.RenameColumn(
                name: "InstructorID",
                table: "UserCourseConnection",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "InstructorCourseConnectionId",
                table: "UserCourseConnection",
                newName: "UserCourseConnectionId");

            migrationBuilder.RenameIndex(
                name: "IX_InstructorCourseConnection_InstructorID",
                table: "UserCourseConnection",
                newName: "IX_UserCourseConnection_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_InstructorCourseConnection_CourseId",
                table: "UserCourseConnection",
                newName: "IX_UserCourseConnection_CourseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserCourseConnection",
                table: "UserCourseConnection",
                column: "UserCourseConnectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserCourseConnection_Courses_CourseId",
                table: "UserCourseConnection",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCourseConnection_User_UserId",
                table: "UserCourseConnection",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
