using System.Security.Claims;
using System.Threading.Tasks;
using Entities;
using Microsoft.AspNet.Mvc;
using StackExchange.Redis;

namespace Forums.Controllers.Api
{
    [Route("api/HierarchyPosts")]
    [Produces("application/json")]
    public class HierarchyPostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PostsCacher _postsCacher;
        private readonly ConnectionMultiplexer _redis;

        public HierarchyPostsController(ApplicationDbContext context, PostsCacher postsCacher, ConnectionMultiplexer redis)
        {
            _context = context;
            _postsCacher = postsCacher;
            _redis = redis;
        }

        // GET: api/HierarchyPost/5
        [HttpGet("{id}", Name = "GetHierarchyPost")]
        public async Task<IActionResult> GetHierarchyPost([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }
            var key = id.ToString();
            var userId = HttpContext.User?.GetUserId();
            _redis.GetSubscriber().Publish(key, $"Post {id} was requested!");
            var value = _redis.GetDatabase().StringIncrement("a");
            var a = value;
            if (userId != null)
            {
                _redis.GetSubscriber().Publish(key, $"UserId {userId} requested post {id}!");
            }
            _postsCacher.IncreaseViewCountAsync(id);
            var gzippedPosts = await _postsCacher.GetGzipPostFromRedisAsync(id);

            if (gzippedPosts == null)
            {
                return HttpNotFound();
            }
            
            Response.Headers.Add("Content-Encoding", "gzip");
            //Response.Headers.Add("X-Content-Encoding", "gzip");
            return File(gzippedPosts, "application/json", null);
        }

    }
}