using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Infrastructure.Persistence.Migrations
{
    public partial class add_new_company_claims : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "UserClaimTypes",
                columns: new[] { "Id", "Name", "Value" },
                values: new object[] { 15, "Company.Any.SetManagerFromSelfToNone", "Company.Any.SetManagerFromSelfToNone" });

            migrationBuilder.InsertData(
                table: "UserClaimTypes",
                columns: new[] { "Id", "Name", "Value" },
                values: new object[] { 16, "Company.New.SetManagerToAny", "Company.New.SetManagerToAny" });

            migrationBuilder.InsertData(
                table: "UserClaimTypes",
                columns: new[] { "Id", "Name", "Value" },
                values: new object[] { 17, "Company.New.SetManagerToSelf", "Company.New.SetManagerToSelf" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 17);
        }
    }
}
