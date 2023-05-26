using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Infrastructure.Persistence.Migrations
{
    public partial class Add_manager_claims : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "UserClaimTypes",
                columns: new[] { "Id", "Name", "Value" },
                values: new object[] { 8, "Company.Any.Manager.Any.Set.Any", "company.any.manager.any.set.any" });

            migrationBuilder.InsertData(
                table: "UserClaimTypes",
                columns: new[] { "Id", "Name", "Value" },
                values: new object[] { 9, "Company.Any.Manager.Any.Set.Self", "company.any.manager.any.set.self" });

            migrationBuilder.InsertData(
                table: "UserClaimTypes",
                columns: new[] { "Id", "Name", "Value" },
                values: new object[] { 10, "Company.Any.Manager.None.Set.Self", "company.any.manager.none.set.self" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 10);
        }
    }
}
