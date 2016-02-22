using System.Threading.Tasks;
using Entities;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;

namespace Forums.Controllers.Api
{
    [Route("api/HierarchyPosts")]
    [Produces("application/json")]
    public class HierarchyPostsController : Controller
    {
        private ApplicationDbContext _context;

        public HierarchyPostsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/HierarchyPost/5
        [HttpGet("{id}", Name = "GetHierarchyPost")]

        public async Task<IActionResult> GetHierarchyPost([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var posts = await _context.GeHierarchyPost(id).ToListAsync();

            if (posts.Count == 0)
            {
                return HttpNotFound();
            }

            return Ok(posts);
        }

    }
}