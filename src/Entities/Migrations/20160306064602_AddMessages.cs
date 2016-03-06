using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;
using Microsoft.Data.Entity.Metadata;

namespace Entities.Migrations
{
    public partial class AddMessages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Body = table.Column<string>(nullable: true),
                    IsRead = table.Column<bool>(nullable: false),
                    IsRecipientDeleted = table.Column<bool>(nullable: false),
                    IsSenderDeleted = table.Column<bool>(nullable: false),
                    RecipientId = table.Column<string>(nullable: false),
                    SenderId = table.Column<string>(nullable: false),
                    SentDate = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Message", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Message_ApplicationUser_RecipientId",
                        column: x => x.RecipientId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Message_ApplicationUser_SenderId",
                        column: x => x.SenderId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });
            migrationBuilder.CreateIndex(
                name: "IX_Message_IsRecipientDeleted_RecipientId",
                table: "Messages",
                columns: new[] { "IsRecipientDeleted", "RecipientId" });
            migrationBuilder.CreateIndex(
                name: "IX_Message_IsSenderDeleted_SenderId",
                table: "Messages",
                columns: new[] { "IsSenderDeleted", "SenderId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("Messages");
        }
    }
}
