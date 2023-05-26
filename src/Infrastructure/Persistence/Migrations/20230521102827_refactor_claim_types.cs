using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Infrastructure.Persistence.Migrations
{
    public partial class refactor_claim_types : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.Any.SetManagerFromAnyToNone", "Company.Any.SetManagerFromAnyToNone" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.New.SetManagerToSelf", "Company.New.SetManagerToSelf" });

            migrationBuilder.InsertData(
                table: "UserClaimTypes",
                columns: new[] { "Id", "Name", "Value" },
                values: new object[] { 14, "Company.New.SetManagerToNone", "Company.New.SetManagerToNone" });

            migrationBuilder.InsertData(
                table: "UserClaimTypes",
                columns: new[] { "Id", "Name", "Value" },
                values: new object[] { 15, "Company.New.SetManagerToAny", "Company.New.SetManagerToAny" });
        }
    }
}
