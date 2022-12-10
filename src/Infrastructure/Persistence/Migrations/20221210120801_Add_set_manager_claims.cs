using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Infrastructure.Persistence.Migrations
{
    public partial class Add_set_manager_claims : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "UserClaimTypes",
                columns: new[] { "Id", "Name", "Value" },
                values: new object[,]
                {
                    { 8, "Company. Manager. Assign anyone from anyone", "company.manager.set.anyFromAny" },
                    { 9, "Company. Manager. Assign anyone from none", "company.manager.set.anyFromNone" },
                    { 10, "Company. Manager. Assign anyone from self", "company.manager.set.anyFromSelf" },
                    { 11, "Company. Manager. Assign none from anyone", "company.manager.set.noneFromAny" },
                    { 12, "Company. Manager. Assign none from self", "company.manager.set.noneFromSelf" },
                    { 13, "Company. Manager. Assign self from anyone", "company.manager.set.selfFromAny" },
                    { 14, "Company. Manager. Assign self from none", "company.manager.set.selfFromNone" }
                });
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

            migrationBuilder.DeleteData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 14);
        }
    }
}
