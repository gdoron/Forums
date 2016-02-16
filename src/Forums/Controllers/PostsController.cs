using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;
using Entities;
using Forums.Models;

namespace Forums.Controllers
{
    public class PostsController : Controller
    {
        private ApplicationDbContext _context;

        public PostsController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: Posts
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Posts.Include(p => p.Forum).Include(p => p.ReplyToPost).Include(p => p.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Post post = await _context.Posts.SingleAsync(m => m.Id == id);
            if (post == null)
            {
                return HttpNotFound();
            }

            return View(post);
        }

        // GET: Posts/Create
        public IActionResult Create()
        {
            ViewData["ForumId"] = new SelectList(_context.Forums, "Id", "Name");
            ViewData["ReplyToPostId"] = new SelectList(_context.Posts, "ReplyToPostId", "ReplyToPostId");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "UserName");
            return View();
        }

        // POST: Posts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Post post)
        {
            if (ModelState.IsValid)
            {
                _context.Posts.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["ForumId"] = new SelectList(_context.Forums, "Id", "Forum", post.ForumId);
            ViewData["ReplyToPostId"] = new SelectList(_context.Posts, "Id", "ReplyToPost", post.ReplyToPostId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "User", post.UserId);
            return View(post);
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Post post = await _context.Posts.SingleAsync(m => m.Id == id);
            if (post == null)
            {
                return HttpNotFound();
            }
            ViewData["ForumId"] = new SelectList(_context.Forums, "Id", "Forum", post.ForumId);
            ViewData["ReplyToPostId"] = new SelectList(_context.Posts, "Id", "ReplyToPost", post.ReplyToPostId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "User", post.UserId);
            return View(post);
        }

        // POST: Posts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Post post)
        {
            if (ModelState.IsValid)
            {
                _context.Update(post);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["ForumId"] = new SelectList(_context.Forums, "Id", "Forum", post.ForumId);
            ViewData["ReplyToPostId"] = new SelectList(_context.Posts, "Id", "ReplyToPost", post.ReplyToPostId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "User", post.UserId);
            return View(post);
        }

        // GET: Posts/Delete/5
        [ActionName("Delete")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Post post = await _context.Posts.SingleAsync(m => m.Id == id);
            if (post == null)
            {
                return HttpNotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Post post = await _context.Posts.SingleAsync(m => m.Id == id);
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
