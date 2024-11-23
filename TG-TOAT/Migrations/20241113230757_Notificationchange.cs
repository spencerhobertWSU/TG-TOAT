using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TGTOAT.Migrations
{
    /// <inheritdoc />
    public partial class Notificationchange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
       name: "Notifications");

            // Create the Notifications table again (with any updated schema)
            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            // Optionally, insert data into the Notifications table if needed
            migrationBuilder.InsertData(
                table: "Notifications",
                columns: new[] { "Id", "Message", "CreatedAt", "IsRead", "StudentId" },
                values: new object[,]
                {
            { 1, "Test Message 1", DateTime.Now, false, 1 },
            { 2, "Test Message 2", DateTime.Now, false, 2 }
                    // Add more data as needed
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
           
            migrationBuilder.DropTable(
                name: "Notifications");


        }
    }
}
