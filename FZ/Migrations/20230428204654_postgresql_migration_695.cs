using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FZ.Migrations
{
    public partial class postgresql_migration_695 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Site",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false),
                    RetryCount = table.Column<int>(type: "integer", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Site", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Site",
                columns: new[] { "Id", "Active", "Message", "RetryCount", "Url" },
                values: new object[] { new Guid("547db874-e087-47eb-99de-e2bf05c05b4a"), true, "root", 0, "root" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Site");
        }
    }
}
