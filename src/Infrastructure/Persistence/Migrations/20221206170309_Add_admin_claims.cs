using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Infrastructure.Persistence.Migrations
{
    public partial class Add_admin_claims : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "UserClaimTypes",
                columns: new[] { "Id", "Name", "Value" },
                values: new object[] { 5, "Company. View any", "company.view.any" });

            migrationBuilder.InsertData(
                table: "UserClaimTypes",
                columns: new[] { "Id", "Name", "Value" },
                values: new object[] { 6, "Company. Delete any", "company.delete.any" });

            migrationBuilder.InsertData(
                table: "UserClaimTypes",
                columns: new[] { "Id", "Name", "Value" },
                values: new object[] { 7, "Company. Update any", "company.update.any" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 7);
        }
    }
}
