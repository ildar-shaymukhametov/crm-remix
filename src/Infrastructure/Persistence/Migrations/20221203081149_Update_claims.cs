using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Infrastructure.Persistence.Migrations
{
    public partial class Update_claims : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Company. Create");

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Company. Update");

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Company. Delete");

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "Company. View");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Компания. Добавление");

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Компания. Редактирование");

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Компания. Удаление");

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "Компания. Просмотр");
        }
    }
}
