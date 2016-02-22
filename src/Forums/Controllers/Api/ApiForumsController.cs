using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;

namespace Forums.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/Forums")]
    [Authorize(Roles = "Admin")]
    public class ApiForumsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ApiForumsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/ApiForums
        [HttpGet]
        [AllowAnonymous]
        public IEnumerable<Forum> GetForums()
        {
            return _context.Forums;
        }

        // GET: api/ApiForums/5
        [HttpGet("{id}", Name = "GetForum")]
        [AllowAnonymous]
        public async Task<IActionResult> GetForum([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var forum = await _context.Forums.SingleAsync(m => m.Id == id);

            if (forum == null)
            {
                return HttpNotFound();
            }

            return Ok(forum);
        }

        // PUT: api/ApiForums/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutForum([FromRoute] int id, [FromBody] Forum forum)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            if (id != forum.Id)
            {
                return HttpBadRequest();
            }

            _context.Entry(forum).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ForumExists(id))
                {
                    return HttpNotFound();
                }
                throw;
            }

            return new HttpStatusCodeResult(StatusCodes.Status204NoContent);
        }

        // POST: api/ApiForums
        [HttpPost]
        public async Task<IActionResult> PostForum([FromBody] Forum forum)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            _context.Forums.Add(forum);

            var roleName = $"Forum-{forum.Name}-Manager";
            var newRole = new IdentityRole {Name = roleName, NormalizedName = roleName.ToUpper()};
            _context.Roles.Add(newRole);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ForumExists(forum.Id))
                {
                    return new HttpStatusCodeResult(StatusCodes.Status409Conflict);
                }

                throw;
            }

            return CreatedAtRoute("GetForum", new {id = forum.Id}, forum);
        }

        // DELETE: api/ApiForums/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteForum([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var forum = await _context.Forums.SingleAsync(m => m.Id == id);
            if (forum == null)
            {
                return HttpNotFound();
            }

            _context.Forums.Remove(forum);
            await _context.SaveChangesAsync();

            return Ok(forum);
        }
        [HttpPost]
        [Route("AddManagerToForum")]
        public async Task<IActionResult> AddManagerToForum([FromBody]AddManagerVm model)
        {
            var forumName = (await _context.Forums.SingleAsync(x => x.Id == model.ForumId)).Name;
            var roleName = $"Forum-{forumName}-Manager";
            await _userManager.AddToRoleAsync(await _context.Users.SingleAsync(x => x.Id == model.UserId), roleName);

            return Ok();
        }


        private bool ForumExists(int id)
        {
            return _context.Forums.Count(e => e.Id == id) > 0;
        }
    }

    public class AddManagerVm
    {
        public int ForumId { get; set; }
        public string UserId { get; set; }
    }
}

