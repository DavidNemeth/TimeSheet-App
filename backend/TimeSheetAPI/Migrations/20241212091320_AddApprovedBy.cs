using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeSheet.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddApprovedBy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApprovedBy",
                table: "TimesheetEntries",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                table: "TimesheetEntries");
        }
    }
}
