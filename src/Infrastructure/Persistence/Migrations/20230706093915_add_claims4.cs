using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CRM.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class add_claims4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.New.Other.Set", "Company.New.Other.Set" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.New.Manager.SetToAny", "Company.New.Manager.SetToAny" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.New.Manager.SetToNone", "Company.New.Manager.SetToNone" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.New.Manager.SetToSelf", "Company.New.Manager.SetToSelf" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.Any.Delete", "Company.Any.Delete" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.Any.Other.Get", "Company.Any.Other.Get" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.Any.Other.Set", "Company.Any.Other.Set" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.Any.Name.Get", "Company.Any.Name.Get" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.Any.Name.Set", "Company.Any.Name.Set" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.Any.Manager.Get", "Company.Any.Manager.Get" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.Any.Manager.SetFromAnyToAny", "Company.Any.Manager.SetFromAnyToAny" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.Any.Manager.SetFromAnyToSelf", "Company.Any.Manager.SetFromAnyToSelf" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.Any.Manager.SetFromNoneToSelf", "Company.Any.Manager.SetFromNoneToSelf" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.Any.Manager.SetFromNoneToAny", "Company.Any.Manager.SetFromNoneToAny" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.Any.Manager.SetFromAnyToNone", "Company.Any.Manager.SetFromAnyToNone" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.Any.Manager.SetFromSelfToAny", "Company.Any.Manager.SetFromSelfToAny" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.Any.Manager.SetFromSelfToNone", "Company.Any.Manager.SetFromSelfToNone" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.WhereUserIsManager.Delete", "Company.WhereUserIsManager.Delete" });

            migrationBuilder.InsertData(
                table: "UserClaimTypes",
                columns: new[] { "Id", "Name", "Value" },
                values: new object[,]
                {
                    { 20, "Company.WhereUserIsManager.Other.Get", "Company.WhereUserIsManager.Other.Get" },
                    { 21, "Company.WhereUserIsManager.Other.Set", "Company.WhereUserIsManager.Other.Set" },
                    { 22, "Company.WhereUserIsManager.Name.Get", "Company.WhereUserIsManager.Name.Get" },
                    { 23, "Company.WhereUserIsManager.Name.Set", "Company.WhereUserIsManager.Name.Set" },
                    { 24, "Company.WhereUserIsManager.Manager.Get", "Company.WhereUserIsManager.Manager.Get" },
                    { 25, "Company.WhereUserIsManager.Manager.SetFromSelfToAny", "Company.WhereUserIsManager.Manager.SetFromSelfToAny" },
                    { 26, "Company.WhereUserIsManager.Manager.SetFromSelfToNone", "Company.WhereUserIsManager.Manager.SetFromSelfToNone" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.Any.View", "Company.Any.View" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.Any.Update", "Company.Any.Update" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.Any.Delete", "Company.Any.Delete" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.Any.Other.View", "Company.Any.Other.View" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.Any.Other.Update", "Company.Any.Other.Update" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.Any.Manager.SetFromAnyToAny", "Company.Any.Manager.SetFromAnyToAny" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.Any.Manager.SetFromAnyToSelf", "Company.Any.Manager.SetFromAnyToSelf" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.Any.Manager.SetFromNoneToSelf", "Company.Any.Manager.SetFromNoneToSelf" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.Any.Manager.SetFromNoneToAny", "Company.Any.Manager.SetFromNoneToAny" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.Any.Manager.SetFromAnyToNone", "Company.Any.Manager.SetFromAnyToNone" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.Any.Manager.SetFromSelfToAny", "Company.Any.Manager.SetFromSelfToAny" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.Any.Manager.SetFromSelfToNone", "Company.Any.Manager.SetFromSelfToNone" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.WhereUserIsManager.Update", "Company.WhereUserIsManager.Update" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.WhereUserIsManager.Delete", "Company.WhereUserIsManager.Delete" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.WhereUserIsManager.Other.View", "Company.WhereUserIsManager.Other.View" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.WhereUserIsManager.Other.Update", "Company.WhereUserIsManager.Other.Update" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.WhereUserIsManager.Manager.SetFromSelfToAny", "Company.WhereUserIsManager.Manager.SetFromSelfToAny" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.WhereUserIsManager.Manager.SetFromSelfToNone", "Company.WhereUserIsManager.Manager.SetFromSelfToNone" });
        }
    }
}
