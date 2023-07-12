﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class add_claims3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.InsertData(
                table: "UserClaimTypes",
                columns: new[] { "Id", "Name", "Value" },
                values: new object[] { 19, "Company.WhereUserIsManager.Manager.SetFromSelfToNone", "Company.WhereUserIsManager.Manager.SetFromSelfToNone" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 19);

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
                values: new object[] { "Company.Any.Other.View", "Company.Any.Other.View" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.Any.Other.Update", "Company.Any.Other.Update" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.Any.Manager.SetFromAnyToAny", "Company.Any.Manager.SetFromAnyToAny" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.Any.Manager.SetFromAnyToSelf", "Company.Any.Manager.SetFromAnyToSelf" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.Any.Manager.SetFromNoneToSelf", "Company.Any.Manager.SetFromNoneToSelf" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.Any.Manager.SetFromNoneToAny", "Company.Any.Manager.SetFromNoneToAny" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.Any.Manager.SetFromAnyToNone", "Company.Any.Manager.SetFromAnyToNone" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.Any.Manager.SetFromSelfToAny", "Company.Any.Manager.SetFromSelfToAny" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.Any.Manager.SetFromSelfToNone", "Company.Any.Manager.SetFromSelfToNone" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.WhereUserIsManager.Update", "Company.WhereUserIsManager.Update" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.WhereUserIsManager.Delete", "Company.WhereUserIsManager.Delete" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.WhereUserIsManager.Other.View", "Company.WhereUserIsManager.Other.View" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.WhereUserIsManager.Other.Update", "Company.WhereUserIsManager.Other.Update" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.WhereUserIsManager.Manager.SetFromSelfToAny", "Company.WhereUserIsManager.Manager.SetFromSelfToAny" });

            migrationBuilder.UpdateData(
                table: "UserClaimTypes",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "Name", "Value" },
                values: new object[] { "Company.WhereUserIsManager.Manager.SetFromSelfToNone", "Company.WhereUserIsManager.Manager.SetFromSelfToNone" });
        }
    }
}
