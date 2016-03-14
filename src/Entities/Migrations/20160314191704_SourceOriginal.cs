using Microsoft.Data.Entity.Migrations;

namespace Entities.Migrations
{
    public partial class SourceOriginal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsOriginal",
                table: "Posts",
                nullable: false,
                defaultValue: false);
            migrationBuilder.AddColumn<string>(
                name: "SourceLink",
                table: "Posts",
                nullable: true);

            migrationBuilder.Sql(@"Alter view HierarchyPosts as
WITH Cte (Id, Parentpostid, Depth, Rootid) AS
    (SELECT Id,
            Replytopostid,
            0 AS Thelevel,
            Id AS Rootid
     FROM Posts
     WHERE Replytopostid IS NULL
         AND Isdeleted = 0
     UNION ALL SELECT Pn.Id,
                      Pn.Replytopostid,
                      P1.Depth +1,
                      P1.Rootid
     FROM Posts Pn
     INNER JOIN Cte AS P1 ON P1.Id = Pn.Replytopostid
     WHERE Pn.Isdeleted = 0)
SELECT Cte.Id AS Postid,
       ReplyToPostId,
       Depth,
       ForumId,
       LastChangedDate,
       PublishDate,
       Title,
       Body,
       IsModified,
       U.UserName,
       U.Id,
       UserId,
       RootId,
       IsDeleted,
       IsImportantReply,
       Views,
       IsOriginal,
       SourceLink,   
    (SELECT Count(1)
     FROM Userreviews Ur
     WHERE Ur.Touserid = P.Userid
         AND Ur.Votetype = 1
         AND Ur.IsDeleted = 0) AS PositiveReviewsScore,
    (SELECT Count(1)
     FROM Userreviews Ur
     WHERE Ur.Touserid = P.Userid
         AND Ur.Votetype = 2
         AND Ur.IsDeleted = 0) AS NegativeReviewsScore
FROM Cte
INNER JOIN Posts P ON Cte.Id = P.Id
INNER JOIN Users U ON U.Id = P.Userid");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"Alter view HierarchyPosts as
WITH Cte (Id, Parentpostid, Depth, Rootid) AS
    (SELECT Id,
            Replytopostid,
            0 AS Thelevel,
            Id AS Rootid
     FROM Posts
     WHERE Replytopostid IS NULL
         AND Isdeleted = 0
     UNION ALL SELECT Pn.Id,
                      Pn.Replytopostid,
                      P1.Depth +1,
                      P1.Rootid
     FROM Posts Pn
     INNER JOIN Cte AS P1 ON P1.Id = Pn.Replytopostid
     WHERE Pn.Isdeleted = 0)
SELECT Cte.Id AS Postid,
       ReplyToPostId,
       Depth,
       ForumId,
       LastChangedDate,
       PublishDate,
       Title,
       Body,
       IsModified,
       U.UserName,
       U.Id,
       UserId,
       RootId,
       IsDeleted,
       IsImportantReply,
       Views,
    (SELECT Count(1)
     FROM Userreviews Ur
     WHERE Ur.Touserid = P.Userid
         AND Ur.Votetype = 1
         AND Ur.IsDeleted = 0) AS PositiveReviewsScore,
    (SELECT Count(1)
     FROM Userreviews Ur
     WHERE Ur.Touserid = P.Userid
         AND Ur.Votetype = 2
         AND Ur.IsDeleted = 0) AS NegativeReviewsScore
FROM Cte
INNER JOIN Posts P ON Cte.Id = P.Id
INNER JOIN Users U ON U.Id = P.Userid");
            migrationBuilder.DropColumn(name: "IsOriginal", table: "Posts");
            migrationBuilder.DropColumn(name: "SourceLink", table: "Posts");
        }
    }
}
