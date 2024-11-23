using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TGTOAT.Migrations
{
    /// <inheritdoc />
    public partial class IntstructorConnectionfix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InstructorCourseConnectionId",
                table: "InstructorConnection",
                newName: "InstructorCourseConnectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InstructorCourseConnectId",
                table: "InstructorConnection",
                newName: "InstructorCourseConnectionId");
        }
    }
}
