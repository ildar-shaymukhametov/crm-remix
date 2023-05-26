using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Infrastructure.Persistence.Migrations
{
    public partial class Add_claim_types : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "UserClaimTypes",
                columns: new[] { "Id", "Name", "Value" },
                values: new object[] { 13, "Company.New.SetManagerToSelf", "Company.New.SetManagerToSelf" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 13);
        }
    }
}
