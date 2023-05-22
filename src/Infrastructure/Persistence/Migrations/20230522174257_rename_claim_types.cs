using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Infrastructure.Persistence.Migrations
{
    public partial class rename_claim_types : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.Old.Any.Update", "Company.Old.Any.Update" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.Old.Any.Delete", "Company.Old.Any.Delete" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.Old.Any.View", "Company.Old.Any.View" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.Old.WhereUserIsManager.Update", "Company.Old.WhereUserIsManager.Update" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.Old.WhereUserIsManager.Delete", "Company.Old.WhereUserIsManager.Delete" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.Old.WhereUserIsManager.View", "Company.Old.WhereUserIsManager.View" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.Old.Any.SetManagerFromAnyToAny", "Company.Old.Any.SetManagerFromAnyToAny" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.Old.Any.SetManagerFromAnyToSelf", "Company.Old.Any.SetManagerFromAnyToSelf" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.Old.Any.SetManagerFromNoneToSelf", "Company.Old.Any.SetManagerFromNoneToSelf" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.Old.Any.SetManagerFromNoneToAny", "Company.Old.Any.SetManagerFromNoneToAny" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.Old.WhereUserIsManager.SetManagerFromSelfToAny", "Company.Old.WhereUserIsManager.SetManagerFromSelfToAny" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.Old.Any.SetManagerFromAnyToNone", "Company.Old.Any.SetManagerFromAnyToNone" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.Old.Any.SetManagerFromSelfToAny", "Company.Old.Any.SetManagerFromSelfToAny" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.Old.Any.SetManagerFromSelfToNone", "Company.Old.Any.SetManagerFromSelfToNone" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.WhereUserIsManager.SetManagerFromSelfToAny", "Company.WhereUserIsManager.SetManagerFromSelfToAny" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.Any.SetManagerFromAnyToNone", "Company.Any.SetManagerFromAnyToNone" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.Any.SetManagerFromSelfToAny", "Company.Any.SetManagerFromSelfToAny" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.Any.SetManagerFromSelfToNone", "Company.Any.SetManagerFromSelfToNone" });
        }
    }
}
