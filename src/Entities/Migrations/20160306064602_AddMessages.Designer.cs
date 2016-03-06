using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations;
using Entities;

namespace Entities.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20160306064602_AddMessages")]
    partial class AddMessages
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0-rc1-16348")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Entities.ApplicationUser", b =>
                {
                    b.Property<string>("Id");

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedUserName")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasAnnotation("Relational:Name", "EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .HasAnnotation("Relational:Name", "UserNameIndex");

                    b.HasAnnotation("Relational:TableName", "Users");
                });

            modelBuilder.Entity("Entities.Forum", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "Forums");
                });

            modelBuilder.Entity("Entities.HierarchyPost", b =>
                {
                    b.Property<int>("PostId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Body");

                    b.Property<int>("Depth");

                    b.Property<int>("ForumId");

                    b.Property<bool>("IsDeleted");

                    b.Property<bool>("IsImportantReply");

                    b.Property<bool>("IsModified");

                    b.Property<DateTime?>("LastChangedDate");

                    b.Property<int>("NegativeReviewsScore");

                    b.Property<int>("PositiveReviewsScore");

                    b.Property<DateTime>("PublishDate");

                    b.Property<int?>("ReplyToPostId");

                    b.Property<int>("RootId");

                    b.Property<string>("Title");

                    b.Property<string>("UserId");

                    b.Property<string>("UserName");

                    b.HasKey("PostId");

                    b.HasAnnotation("Relational:TableName", "HierarchyPosts");
                });

            modelBuilder.Entity("Entities.Message", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Body")
                        .HasAnnotation("MaxLength", 1000);

                    b.Property<bool>("IsRead");

                    b.Property<bool>("IsRecipientDeleted");

                    b.Property<bool>("IsSenderDeleted");

                    b.Property<string>("RecipientId")
                        .IsRequired();

                    b.Property<string>("SenderId")
                        .IsRequired();

                    b.Property<DateTime>("SentDate")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("Relational:GeneratedValueSql", "getutcdate()");

                    b.Property<string>("Title")
                        .HasAnnotation("MaxLength", 200);

                    b.HasKey("Id");

                    b.HasIndex("IsRecipientDeleted", "RecipientId");

                    b.HasIndex("IsSenderDeleted", "SenderId");

                    b.HasAnnotation("Relational:TableName", "Messages");
                });

            modelBuilder.Entity("Entities.Post", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Body");

                    b.Property<int>("ForumId");

                    b.Property<bool>("IsDeleted");

                    b.Property<bool>("IsImportantReply");

                    b.Property<bool>("IsLocked");

                    b.Property<bool>("IsModified");

                    b.Property<DateTime?>("LastChangedDate");

                    b.Property<string>("LockReason");

                    b.Property<string>("LockingUserId");

                    b.Property<int>("PostType");

                    b.Property<DateTime>("PublishDate")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("Relational:GeneratedValueSql", "getutcdate()");

                    b.Property<int?>("ReplyToPostId");

                    b.Property<int>("Score");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 400);

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("ForumId", "PublishDate", "LastChangedDate");

                    b.HasAnnotation("Relational:TableName", "Posts");
                });

            modelBuilder.Entity("Entities.PostRevision", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Body");

                    b.Property<string>("ChangingUserId");

                    b.Property<DateTime>("CreationDate");

                    b.Property<int>("PostId");

                    b.Property<string>("Title")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "PostRevisions");
                });

            modelBuilder.Entity("Entities.UserReview", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreationDate")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("Relational:GeneratedValueSql", "getutcdate()");

                    b.Property<string>("FromUserId")
                        .IsRequired();

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("Review");

                    b.Property<string>("ToUserId")
                        .IsRequired();

                    b.Property<DateTime>("UpdateDate")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("Relational:GeneratedValueSql", "getutcdate()");

                    b.Property<int>("VoteType");

                    b.HasKey("Id");

                    b.HasIndex("FromUserId", "ToUserId")
                        .IsUnique();

                    b.HasAnnotation("Relational:TableName", "UserReviews");
                });

            modelBuilder.Entity("Entities.Vote", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreationDate")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("Relational:GeneratedValueSql", "getutcdate()");

                    b.Property<int>("PostId");

                    b.Property<int>("UserId");

                    b.Property<string>("UserId1");

                    b.Property<int>("VoteType");

                    b.HasKey("Id");

                    b.HasIndex("PostId");

                    b.HasAnnotation("Relational:TableName", "Votes");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityRole", b =>
                {
                    b.Property<string>("Id");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .HasAnnotation("Relational:Name", "RoleNameIndex");

                    b.HasAnnotation("Relational:TableName", "Roles");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "RoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "UserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasAnnotation("Relational:TableName", "UserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasAnnotation("Relational:TableName", "UserRoles");
                });

            modelBuilder.Entity("Entities.Message", b =>
                {
                    b.HasOne("Entities.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("RecipientId");

                    b.HasOne("Entities.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("SenderId");
                });

            modelBuilder.Entity("Entities.Post", b =>
                {
                    b.HasOne("Entities.Forum")
                        .WithMany()
                        .HasForeignKey("ForumId");

                    b.HasOne("Entities.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("LockingUserId");

                    b.HasOne("Entities.Post")
                        .WithMany()
                        .HasForeignKey("ReplyToPostId");

                    b.HasOne("Entities.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Entities.PostRevision", b =>
                {
                    b.HasOne("Entities.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("ChangingUserId");

                    b.HasOne("Entities.Post")
                        .WithMany()
                        .HasForeignKey("PostId");
                });

            modelBuilder.Entity("Entities.UserReview", b =>
                {
                    b.HasOne("Entities.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("FromUserId");

                    b.HasOne("Entities.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("ToUserId");
                });

            modelBuilder.Entity("Entities.Vote", b =>
                {
                    b.HasOne("Entities.Post")
                        .WithMany()
                        .HasForeignKey("PostId");

                    b.HasOne("Entities.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId1");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNet.Identity.EntityFramework.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Entities.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Entities.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNet.Identity.EntityFramework.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId");

                    b.HasOne("Entities.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId");
                });
        }
    }
}
