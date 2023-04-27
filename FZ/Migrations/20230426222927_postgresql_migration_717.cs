using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FZ.Migrations
{
    public partial class postgresql_migration_717 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Site",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    url = table.Column<string>(type: "text", nullable: false),
                    RetryCount = table.Column<int>(type: "integer", nullable: false),
                    message = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Site", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Site",
                columns: new[] { "Id", "RetryCount", "message", "url" },
                values: new object[] { new Guid("99a76a1a-2370-470c-9510-85b42cce4472"), 0, "root", "root" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Site");
        }
    }
}
