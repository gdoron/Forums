using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace Entities.Migrations
{
    public partial class AddedViews : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Views",
                table: "Posts",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Views", table: "Posts");
        }
    }
}
