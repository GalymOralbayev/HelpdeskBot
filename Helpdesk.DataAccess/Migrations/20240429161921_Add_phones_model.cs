using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Helpdesk.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Add_phones_model : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PhoneReferences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Position = table.Column<string>(type: "text", nullable: true),
                    Department = table.Column<string>(type: "text", nullable: true),
                    FullName = table.Column<string>(type: "text", nullable: true),
                    InternalNumber = table.Column<long>(type: "bigint", nullable: true),
                    CityNumber = table.Column<long>(type: "bigint", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    SearchColumn = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhoneReferences", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PhoneReferences_SearchColumn",
                table: "PhoneReferences",
                column: "SearchColumn",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PhoneReferences");
        }
    }
}
