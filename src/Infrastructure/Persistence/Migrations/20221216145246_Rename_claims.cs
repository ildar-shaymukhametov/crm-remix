using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Infrastructure.Persistence.Migrations
{
    public partial class Rename_claims : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.Create", "Company.Create" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.Any.Update", "Company.Any.Update" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.Any.Delete", "Company.Any.Delete" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.Any.View", "Company.Any.View" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.WhereUserIsManager.Update", "Company.WhereUserIsManager.Update" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.WhereUserIsManager.Delete", "Company.WhereUserIsManager.Delete" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.WhereUserIsManager.View", "Company.WhereUserIsManager.View" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.Any.SetManagerFromAnyToAny", "Company.Any.SetManagerFromAnyToAny" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.Any.SetManagerFromAnyToSelf", "Company.Any.SetManagerFromAnyToSelf" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.Any.SetManagerFromNoneToSelf", "Company.Any.SetManagerFromNoneToSelf" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.Any.SetManagerFromNoneToAny", "Company.Any.SetManagerFromNoneToAny" });

            migrationBuilder.InsertData(
                table: "UserClaimTypes",
                columns: new[] { "Id", "Name", "Value" },
                values: new object[] { 12, "Company.WhereUserIsManager.SetManagerFromSelfToAny", "Company.WhereUserIsManager.SetManagerFromSelfToAny" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company. Create", "company.create" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company. Update", "company.update" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company. Delete", "company.delete" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company. View", "company.view" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company. View any", "company.view.any" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company. Delete any", "company.delete.any" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company. Update any", "company.update.any" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.Any.Manager.Any.Set.Any", "company.any.manager.any.set.any" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.Any.Manager.Any.Set.Self", "company.any.manager.any.set.self" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.Any.Manager.None.Set.Self", "company.any.manager.none.set.self" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.Any.Manager.None.Set.Any", "company.any.manager.none.set.any" });
        }
    }
}
