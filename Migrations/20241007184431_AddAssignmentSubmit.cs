using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TGTOAT.Migrations
{
    /// <inheritdoc />
    public partial class AddAssignmentSubmit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.AddColumn<string>(
                name: "Submission",
                table: "Assignments",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropColumn(
                name: "Submission",
                table: "Assignments");
        }
    }
}
