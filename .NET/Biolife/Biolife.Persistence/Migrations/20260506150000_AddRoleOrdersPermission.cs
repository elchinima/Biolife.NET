using Biolife.Persistence.Contexts;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Biolife.Persistence.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20260506150000_AddRoleOrdersPermission")]
    public partial class AddRoleOrdersPermission : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Orders",
                table: "Roles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql("UPDATE Roles SET Orders = AdminPanel WHERE AdminPanel = 1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Orders",
                table: "Roles");
        }
    }
}
