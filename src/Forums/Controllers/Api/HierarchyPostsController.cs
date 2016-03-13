using System.Threading.Tasks;
using Entities;
using Microsoft.AspNet.Mvc;

namespace Forums.Controllers.Api
{
    [Route("api/HierarchyPosts")]
    [Produces("application/json")]
    public class HierarchyPostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        //private readonly PostsCacher _postsCacher;
        private readonly PostsCacher2 _postsCacher;

        public HierarchyPostsController(ApplicationDbContext context, PostsCacher2 postsCacher)
        {
            _context = context;
            _postsCacher = postsCacher;
        }

        // GET: api/HierarchyPost/5
        [HttpGet("{id}", Name = "GetHierarchyPost")]
        public async Task<IActionResult> GetHierarchyPost([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
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