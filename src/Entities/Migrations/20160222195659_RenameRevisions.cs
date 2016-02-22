using Microsoft.Data.Entity.Migrations;

namespace Entities.Migrations
{
    public partial class RenameRevisions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "PostRevision",
                newName: "PostRevisions");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "PostRevisions",
                newName: "PostRevision");
        }
    }
}
