using Microsoft.Data.Entity.Migrations;

namespace Entities.Migrations
{
    public partial class UserReviewsHierarchyView : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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
       Replytopostid,
       Depth,
       Forumid,
       Lastchangeddate,
       Publishdate,
       Title,
       Body,
       Ismodified,
       U.Username,
       U.Id,
       Userid,
       Rootid,
       Isdeleted,
       Isimportantreply,
    (SELECT Count(1)
     FROM Userreviews Ur
     WHERE Ur.Touserid = P.Userid
         AND Ur.Votetype = 1) AS PositiveReviewsScore,
    (SELECT Count(1)
     FROM Userreviews Ur
     WHERE Ur.Touserid = P.Userid
         AND Ur.Votetype = 2) AS NegativeReviewsScore
FROM Cte
INNER JOIN Posts P ON Cte.Id = P.Id
INNER JOIN Users U ON U.Id = P.Userid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                                    select cte.Id as PostId, ReplyToPostId, Depth, ForumId, LastChangedDate, PublishDate, Title, Body, IsModified, U.UserName, u.Id as UserId, RootId, IsDeleted, IsImportantReply
                                    from  cte 
                                    INNER JOIN POSTS P ON CTE.ID = P.ID
                                    INNER JOIN USERS u ON U.id = p.UserId");
        }
    }
}