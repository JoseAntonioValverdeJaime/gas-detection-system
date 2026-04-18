using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GasApi.Migrations
{
    public partial class AddAlertLevel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AlertLevel",
                table: "Alerts",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlertLevel",
                table: "Alerts");
        }
    }
}