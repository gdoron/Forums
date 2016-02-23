using System;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations;

namespace Entities.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable("Users", table => new
                                                               {
                                                                   Id = table.Column<string>(nullable: false),
                                                                   AccessFailedCount = table.Column<int>(nullable: false),
                                                                   ConcurrencyStamp = table.Column<string>(nullable: true),
                                                                   Email = table.Column<string>(nullable: true),
                                                                   EmailConfirmed = table.Column<bool>(nullable: false),
                                                                   LockoutEnabled = table.Column<bool>(nullable: false),
                                                                   LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                                                                   NormalizedEmail = table.Column<string>(nullable: true),
                                                                   NormalizedUserName = table.Column<string>(nullable: true),
                                                                   PasswordHash = table.Column<string>(nullable: true),
                                                                   PhoneNumber = table.Column<string>(nullable: true),
                                                                   PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                                                                   SecurityStamp = table.Column<string>(nullable: true),
                                                                   TwoFactorEnabled = table.Column<bool>(nullable: false),
                                                                   UserName = table.Column<string>(nullable: true)
                                                               },
                constraints: table => { table.PrimaryKey("PK_ApplicationUser", x => x.Id); });
            migrationBuilder.CreateTable("Forums", table => new
                                                                {
                                                                    Id = table.Column<int>(nullable: false)
                                                                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                                                                    Description = table.Column<string>(nullable: true),
                                                                    Name = table.Column<string>(nullable: true)
                                                                },
                constraints: table => { table.PrimaryKey("PK_Forum", x => x.Id); });

            migrationBuilder.CreateTable("Roles", table => new
                                                               {
                                                                   Id = table.Column<string>(nullable: false),
                                                                   ConcurrencyStamp = table.Column<string>(nullable: true),
                                                                   Name = table.Column<string>(nullable: true),
                                                                   NormalizedName = table.Column<string>(nullable: true)
                                                               },
                constraints: table => { table.PrimaryKey("PK_IdentityRole", x => x.Id); });
            migrationBuilder.CreateTable("UserClaims", table => new
                                                                    {
                                                                        Id = table.Column<int>(nullable: false)
                                                                            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                                                                        ClaimType = table.Column<string>(nullable: true),
                                                                        ClaimValue = table.Column<string>(nullable: true),
                                                                        UserId = table.Column<string>(nullable: false)
                                                                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUserClaim<string>", x => x.Id);
                    table.ForeignKey("FK_IdentityUserClaim<string>_ApplicationUser_UserId", x => x.UserId, "Users", "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateTable("UserLogins", table => new
                                                                    {
                                                                        LoginProvider = table.Column<string>(nullable: false),
                                                                        ProviderKey = table.Column<string>(nullable: false),
                                                                        ProviderDisplayName = table.Column<string>(nullable: true),
                                                                        UserId = table.Column<string>(nullable: false)
                                                                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUserLogin<string>", x => new {x.LoginProvider, x.ProviderKey});
                    table.ForeignKey("FK_IdentityUserLogin<string>_ApplicationUser_UserId", x => x.UserId, "Users", "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateTable("Posts", table => new
                                                               {
                                                                   Id = table.Column<int>(nullable: false)
                                                                       .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                                                                   Body = table.Column<string>(nullable: true),
                                                                   ForumId = table.Column<int>(nullable: false),
                                                                   IsDeleted = table.Column<bool>(nullable: false),
                                                                   IsImportantReply = table.Column<bool>(nullable: false),
                                                                   IsLocked = table.Column<bool>(nullable: false),
                                                                   IsModified = table.Column<bool>(nullable: false),
                                                                   LastChangedDate = table.Column<DateTime>(nullable: true),
                                                                   LockReason = table.Column<string>(nullable: true),
                                                                   LockingUserId = table.Column<string>(nullable: true),
                                                                   PostType = table.Column<int>(nullable: false),
                                                                   PublishDate = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                                                                   ReplyToPostId = table.Column<int>(nullable: true),
                                                                   Score = table.Column<int>(nullable: false),
                                                                   Title = table.Column<string>(nullable: false),
                                                                   UserId = table.Column<string>(nullable: true)
                                                               },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post", x => x.Id);
                    table.ForeignKey("FK_Post_Forum_ForumId", x => x.ForumId, "Forums", "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey("FK_Post_ApplicationUser_LockingUserId", x => x.LockingUserId, "Users", "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey("FK_Post_Post_ReplyToPostId", x => x.ReplyToPostId, "Posts", "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey("FK_Post_ApplicationUser_UserId", x => x.UserId, "Users", "Id",
                        onDelete: ReferentialAction.Restrict);
                });
            migrationBuilder.CreateTable("RoleClaims", table => new
                                                                    {
                                                                        Id = table.Column<int>(nullable: false)
                                                                            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                                                                        ClaimType = table.Column<string>(nullable: true),
                                                                        ClaimValue = table.Column<string>(nullable: true),
                                                                        RoleId = table.Column<string>(nullable: false)
                                                                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityRoleClaim<string>", x => x.Id);
                    table.ForeignKey("FK_IdentityRoleClaim<string>_IdentityRole_RoleId", x => x.RoleId, "Roles", "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateTable("UserRoles", table => new
                                                                   {
                                                                       UserId = table.Column<string>(nullable: false),
                                                                       RoleId = table.Column<string>(nullable: false)
                                                                   },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUserRole<string>", x => new {x.UserId, x.RoleId});
                    table.ForeignKey("FK_IdentityUserRole<string>_IdentityRole_RoleId", x => x.RoleId, "Roles", "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey("FK_IdentityUserRole<string>_ApplicationUser_UserId", x => x.UserId, "Users", "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateTable("PostRevisions", table => new
                                                                       {
                                                                           Id = table.Column<int>(nullable: false)
                                                                               .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                                                                           Body = table.Column<string>(nullable: true),
                                                                           ChangingUserId = table.Column<string>(nullable: true),
                                                                           CreationDate = table.Column<DateTime>(nullable: false),
                                                                           PostId = table.Column<int>(nullable: false),
                                                                           Title = table.Column<string>(nullable: false)
                                                                       },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostRevision", x => x.Id);
                    table.ForeignKey("FK_PostRevision_ApplicationUser_ChangingUserId", x => x.ChangingUserId, "Users", "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey("FK_PostRevision_Post_PostId", x => x.PostId, "Posts", "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateTable("Votes", table => new
                                                               {
                                                                   Id = table.Column<int>(nullable: false)
                                                                       .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                                                                   CreationDate = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                                                                   PostId = table.Column<int>(nullable: false),
                                                                   UserId = table.Column<int>(nullable: false),
                                                                   UserId1 = table.Column<string>(nullable: true),
                                                                   VoteType = table.Column<int>(nullable: false)
                                                               },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vote", x => x.Id);
                    table.ForeignKey("FK_Vote_Post_PostId", x => x.PostId, "Posts", "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey("FK_Vote_ApplicationUser_UserId1", x => x.UserId1, "Users", "Id",
                        onDelete: ReferentialAction.Restrict);
                });


            migrationBuilder.Sql(@"CREATE view HierarchyPosts as
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
                                    select cte.Id as PostId, ReplyToPostId, Depth, ForumId, LastChangedDate, PublishDate, Title, Body, IsModified, U.UserName, u.Id as UserId, RootId, IsDeleted, IsImportantReply
                                    from  cte 
                                    INNER JOIN POSTS P ON CTE.ID = P.ID
                                    INNER JOIN USERS u ON U.id = p.UserId");

            migrationBuilder.Sql(@"CREATE TRIGGER trigger_Posts_LastChangedDate
                                    ON dbo.Posts
                                    AFTER UPDATE
                                    AS
                                        UPDATE dbo.posts
                                        SET LastChangedDate = GETDATE()
                                        WHERE ID IN (SELECT DISTINCT ID FROM Inserted)");


            migrationBuilder.CreateIndex("EmailIndex", "Users", "NormalizedEmail");
            migrationBuilder.CreateIndex("UserNameIndex", "Users", "NormalizedUserName");
            migrationBuilder.CreateIndex("IX_Post_ForumId_PublishDate_LastChangedDate", "Posts", new[] {"ForumId", "PublishDate", "LastChangedDate"});
            migrationBuilder.CreateIndex("IX_Vote_PostId", "Votes", "PostId");
            migrationBuilder.CreateIndex("RoleNameIndex", "Roles", "NormalizedName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW HierarchyPosts");
            migrationBuilder.Sql("DROP TRIGGER trigger_Posts_LastChangedDate");
            migrationBuilder.DropTable("PostRevisions");
            migrationBuilder.DropTable("Votes");
            migrationBuilder.DropTable("RoleClaims");
            migrationBuilder.DropTable("UserClaims");
            migrationBuilder.DropTable("UserLogins");
            migrationBuilder.DropTable("UserRoles");
            migrationBuilder.DropTable("Posts");
            migrationBuilder.DropTable("Roles");
            migrationBuilder.DropTable("Forums");
            migrationBuilder.DropTable("Users");
        }
    }
}