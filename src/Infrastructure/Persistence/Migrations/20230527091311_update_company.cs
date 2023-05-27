using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class update_company : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Companies");

            migrationBuilder.AddColumn<int>(
                name: "TypeId",
                table: "Companies",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Companies_TypeId",
                table: "Companies",
                column: "TypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_CompanyTypes_TypeId",
                table: "Companies",
                column: "TypeId",
                principalTable: "CompanyTypes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Companies_CompanyTypes_TypeId",
                table: "Companies");

            migrationBuilder.DropIndex(
                name: "IX_Companies_TypeId",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "TypeId",
                table: "Companies");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
