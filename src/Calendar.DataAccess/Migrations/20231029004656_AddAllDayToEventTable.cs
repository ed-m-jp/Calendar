using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Calendar.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddAllDayToEventTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AllDay",
                table: "Events",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllDay",
                table: "Events");
        }
    }
}
