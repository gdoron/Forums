using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Entities;
using Forums.Extensions;
using Forums.Filters;
using Forums.Models;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Caching.Memory;

namespace Forums.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/Posts")]
    [ServiceFilter(typeof (EntityFrameworkFilter))]
    public class ApiPostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly MapperConfiguration _mapperConfiguration;
        private readonly IMemoryCache _memoryCache;
        private readonly PostsCacher _postsCacher;

        public ApiPostsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, MapperConfiguration mapperConfiguration
            ,IMemoryCache memoryCache, PostsCacher postsCacher)
        {
            _context = context;
            _userManager = userManager;
            _mapperConfiguration = mapperConfiguration;
            _memoryCache = memoryCache;
            _postsCacher = postsCacher;
        }

        // GET: api/Posts
        [HttpGet]
        public async Task<IActionResult> GetPosts(int forumId, int pageSize = 50, int page = 0)
        {
            var results = await _context.Posts.Include(x=> x.User)
                .Where(x => x.ForumId == forumId && x.ReplyToPostId == null)
                .Skip(pageSize*page)
                .Take(pageSize)
                //.ProjectTo<PostListViewModel>(_mapperConfiguration)
                .ToListAsync();

            var models = _mapperConfiguration.CreateMapper().Map<List<Post>, List<PostListViewModel>>(results);
            foreach (var model in models)
            {
                int viewsCount = -1;
                var cacheKey = model.Id + ":ViewsCount";
                if (_memoryCache.TryGetValue(cacheKey, out viewsCount))
                    model.ViewCount = viewsCount;
            }

            return Ok(models);
        }

        // GET: api/Posts/5
        [HttpGet("{id}", Name = "GetPost")]
        public async Task<IActionResult> GetPost([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var post = await _context.Posts.SingleAsync(m => m.Id == id);

            if (post == null)
            {
                return HttpNotFound();
            }

            return Ok(post);
        }

        // PUT: api/Posts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPost([FromRoute] int id, [FromBody] Post post)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            if (id != post.Id)
            {
                return HttpBadRequest();
            }

            _context.Entry(post).State = EntityState.Modified;
            post.IsModified = true;
            try
            {
                await _context.SaveChangesAsync();
                await _postsCacher.UpdateCacheAfterPostChange(post.Id);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostExists(id))
                {
                    return HttpNotFound();
                }
                throw;
            }

            return new HttpStatusCodeResult(StatusCodes.Status204NoContent);
        }

        // PUT: api/Posts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] EditPostViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            if (id != model.Id)
            {
                return HttpBadRequest();
            }

            var post = await _context.Posts.SingleOrDefaultAsync(x => x.Id == model.Id);
            post.Body = model.Body.ParseMarkdown();
            post.Title = model.Title.Sanitize();
            post.IsModified = true;

            var changingUser = await GetCurrentUserAsync();
            _context.PostRevisions.Add(new PostRevision
                                           {
                                               Post = post,
                                               Title = model.Title,
                                               Body = model.Body,
                                               ChangingUser = changingUser,
                                               CreationDate = DateTime.UtcNow
                                           });
            try
            {
                await _context.SaveChangesAsync();
                await _postsCacher.UpdateCacheAfterPostChange(model.Id);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostExists(id))
                {
                    return HttpNotFound();
                }
                throw;
            }

            return new HttpStatusCodeResult(StatusCodes.Status204NoContent);
        }

        // POST: api/Posts
        [HttpPost]
        public async Task<IActionResult> PostPost([FromBody] Post post)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            post.Body = post.Body.ParseMarkdown();
            post.Title = post.Title.Sanitize();

            var changingUser = await GetCurrentUserAsync();
            _context.Posts.Add(post);
            _context.PostRevisions.Add(new PostRevision
                                           {
                                               Post = post,
                                               Title = post.Title,
                                               Body = post.Body,
                                               ChangingUser = changingUser,
                                               CreationDate = DateTime.UtcNow
                                           });

            try
            {
                await _context.SaveChangesAsync();
                await _postsCacher.UpdateCacheAfterPostChange(post.Id);
            }
            catch (DbUpdateException)
            {
                if (PostExists(post.Id))
                {
                    return new HttpStatusCodeResult(StatusCodes.Status409Conflict);
                }
                throw;
            }

            return CreatedAtRoute("GetPost", new {id = post.Id}, post);
        }

        // DELETE: api/Posts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var post = await _context.Posts.SingleAsync(m => m.Id == id);
            if (post == null)
            {
                return HttpNotFound();
            }

            post.IsDeleted = true;
            await _context.SaveChangesAsync();
            await _postsCacher.UpdateCacheAfterPostChange(post.Id);

            return Ok(post);
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }

        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            return await _userManager.FindByIdAsync(HttpContext.User.GetUserId());
        }
    }
}