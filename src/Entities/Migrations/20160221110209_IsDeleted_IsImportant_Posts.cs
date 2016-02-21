using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace Entities.Migrations
{
    public partial class IsDeleted_IsImportant_Posts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_Post_Forum_ForumId", table: "Posts");
            migrationBuilder.DropForeignKey(name: "FK_IdentityRoleClaim<string>_IdentityRole_RoleId", table: "RoleClaims");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserClaim<string>_ApplicationUser_UserId", table: "UserClaims");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserLogin<string>_ApplicationUser_UserId", table: "UserLogins");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserRole<string>_IdentityRole_RoleId", table: "UserRoles");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserRole<string>_ApplicationUser_UserId", table: "UserRoles");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Posts",
                nullable: false,
                defaultValue: false);
            migrationBuilder.AddColumn<bool>(
                name: "IsImportantReply",
                table: "Posts",
                nullable: false,
                defaultValue: false);


            migrationBuilder.Sql(@"Alter view HierarchyPosts as
                                    WITH    cte ( Id, ParentPostId, Depth, RootId ) 
				                                    AS ( SELECT   Id,
							                                    ReplyToPostId,
							                                    0 as TheLevel,
							                                    Id as RootId
					                                    FROM     posts
					                                    where ReplyToPostId is null
					                                    And IsDeleted = 0
					                                    UNION ALL 
					                                    SELECT   pn.Id, 
							                                    pn.ReplyToPostId,
							                                    p1.Depth +1,
							                                    p1.RootId
					                                    FROM    Posts pn
					                                    INNER JOIN cte AS p1 on p1.Id = pn.ReplyToPostId
					                                    Where   pn.IsDeleted = 0				
					                                    )
                                    select cte.Id as PostId, ReplyToPostId, Depth, ForumId, LastChangedDate, PublishDate, Text, U.UserName, u.Id as UserId, RootId, IsDeleted, IsImportantReply
                                    from  cte 
                                    INNER JOIN POSTS P ON CTE.ID = P.ID
                                    INNER JOIN USERS u ON U.id = p.UserId
                                    ");

            migrationBuilder.AddForeignKey(
                name: "FK_Post_Forum_ForumId",
                table: "Posts",
                column: "ForumId",
                principalTable: "Forums",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityRoleClaim<string>_IdentityRole_RoleId",
                table: "RoleClaims",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserClaim<string>_ApplicationUser_UserId",
                table: "UserClaims",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserLogin<string>_ApplicationUser_UserId",
                table: "UserLogins",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserRole<string>_IdentityRole_RoleId",
                table: "UserRoles",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserRole<string>_ApplicationUser_UserId",
                table: "UserRoles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_Post_Forum_ForumId", table: "Posts");
            migrationBuilder.DropForeignKey(name: "FK_IdentityRoleClaim<string>_IdentityRole_RoleId", table: "RoleClaims");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserClaim<string>_ApplicationUser_UserId", table: "UserClaims");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserLogin<string>_ApplicationUser_UserId", table: "UserLogins");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserRole<string>_IdentityRole_RoleId", table: "UserRoles");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserRole<string>_ApplicationUser_UserId", table: "UserRoles");

            migrationBuilder.Sql(@"ALTER VIEW HierarchyPosts as
WITH    cte ( Id, ParentPostId, Depth, RootId ) 
				AS ( SELECT   Id,
							ReplyToPostId,
							0 as TheLevel,
							Id as RootId
					FROM     posts
					where ReplyToPostId is null
					UNION ALL 
					SELECT   pn.Id, 
							pn.ReplyToPostId,
							p1.Depth +1,
							p1.RootId
					FROM     Posts pn
					INNER JOIN cte AS p1 on p1.Id = pn.ReplyToPostId
					)
select cte.Id as PostId, ReplyToPostId, Depth, ForumId, LastChangedDate, PublishDate, Text, U.UserName, u.Id as UserId, RootId
from  cte 
INNER JOIN POSTS P ON CTE.ID = P.ID
INNER JOIN USERS u ON U.id = p.UserId");

            migrationBuilder.DropColumn(name: "IsDeleted", table: "Posts");
            migrationBuilder.DropColumn(name: "IsImportantReply", table: "Posts");
            
            migrationBuilder.AddForeignKey(
                name: "FK_Post_Forum_ForumId",
                table: "Posts",
                column: "ForumId",
                principalTable: "Forums",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityRoleClaim<string>_IdentityRole_RoleId",
                table: "RoleClaims",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserClaim<string>_ApplicationUser_UserId",
                table: "UserClaims",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserLogin<string>_ApplicationUser_UserId",
                table: "UserLogins",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserRole<string>_IdentityRole_RoleId",
                table: "UserRoles",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserRole<string>_ApplicationUser_UserId",
                table: "UserRoles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
