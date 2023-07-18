using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ConsoleClient.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class First : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CompanyTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserClaimTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Value = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaimTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IdentityUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityUsers_ApplicationUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Inn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ceo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Contacts = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ManagerId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Companies_ApplicationUsers_ManagerId",
                        column: x => x.ManagerId,
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Companies_CompanyTypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "CompanyTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "IdentityClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdentityUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityClaims_IdentityUsers_IdentityUserId",
                        column: x => x.IdentityUserId,
                        principalTable: "IdentityUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "CompanyTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "ООО" },
                    { 2, "АО" },
                    { 3, "ПАО" },
                    { 4, "ИП" }
                });

            migrationBuilder.InsertData(
                table: "UserClaimTypes",
                columns: new[] { "Id", "Name", "Value" },
                values: new object[,]
                {
                    { 1, "Company.Create", "Company.Create" },
                    { 2, "Company.New.Other.Set", "Company.New.Other.Set" },
                    { 3, "Company.New.Manager.SetToAny", "Company.New.Manager.SetToAny" },
                    { 4, "Company.New.Manager.SetToNone", "Company.New.Manager.SetToNone" },
                    { 5, "Company.New.Manager.SetToSelf", "Company.New.Manager.SetToSelf" },
                    { 6, "Company.Any.Delete", "Company.Any.Delete" },
                    { 7, "Company.Any.Other.Get", "Company.Any.Other.Get" },
                    { 8, "Company.Any.Other.Set", "Company.Any.Other.Set" },
                    { 9, "Company.Any.Name.Get", "Company.Any.Name.Get" },
                    { 10, "Company.Any.Name.Set", "Company.Any.Name.Set" },
                    { 11, "Company.Any.Manager.Get", "Company.Any.Manager.Get" },
                    { 12, "Company.Any.Manager.SetFromAnyToAny", "Company.Any.Manager.SetFromAnyToAny" },
                    { 13, "Company.Any.Manager.SetFromAnyToSelf", "Company.Any.Manager.SetFromAnyToSelf" },
                    { 14, "Company.Any.Manager.SetFromNoneToSelf", "Company.Any.Manager.SetFromNoneToSelf" },
                    { 15, "Company.Any.Manager.SetFromNoneToAny", "Company.Any.Manager.SetFromNoneToAny" },
                    { 16, "Company.Any.Manager.SetFromAnyToNone", "Company.Any.Manager.SetFromAnyToNone" },
                    { 17, "Company.Any.Manager.SetFromSelfToAny", "Company.Any.Manager.SetFromSelfToAny" },
                    { 18, "Company.Any.Manager.SetFromSelfToNone", "Company.Any.Manager.SetFromSelfToNone" },
                    { 19, "Company.WhereUserIsManager.Delete", "Company.WhereUserIsManager.Delete" },
                    { 20, "Company.WhereUserIsManager.Other.Get", "Company.WhereUserIsManager.Other.Get" },
                    { 21, "Company.WhereUserIsManager.Other.Set", "Company.WhereUserIsManager.Other.Set" },
                    { 22, "Company.WhereUserIsManager.Name.Get", "Company.WhereUserIsManager.Name.Get" },
                    { 23, "Company.WhereUserIsManager.Name.Set", "Company.WhereUserIsManager.Name.Set" },
                    { 24, "Company.WhereUserIsManager.Manager.Get", "Company.WhereUserIsManager.Manager.Get" },
                    { 25, "Company.WhereUserIsManager.Manager.SetFromSelfToAny", "Company.WhereUserIsManager.Manager.SetFromSelfToAny" },
                    { 26, "Company.WhereUserIsManager.Manager.SetFromSelfToNone", "Company.WhereUserIsManager.Manager.SetFromSelfToNone" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Companies_ManagerId",
                table: "Companies",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_TypeId",
                table: "Companies",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityClaims_IdentityUserId",
                table: "IdentityClaims",
                column: "IdentityUserId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityUsers_ApplicationUserId",
                table: "IdentityUsers",
                column: "ApplicationUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropTable(
                name: "IdentityClaims");

            migrationBuilder.DropTable(
                name: "UserClaimTypes");

            migrationBuilder.DropTable(
                name: "CompanyTypes");

            migrationBuilder.DropTable(
                name: "IdentityUsers");

            migrationBuilder.DropTable(
                name: "ApplicationUsers");
        }
    }
}
