using System.Linq;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;
using Entities;
using Forums.Models;

namespace Forums.Controllers
{
    public class ForumsController : Controller
    {
        private ApplicationDbContext _context;

        public ForumsController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: Forum
        public IActionResult Index()
        {
            return View(_context.Forums.ToList());
        }

        // GET: Forums/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Forum forum = _context.Forums.Single(m => m.Id == id);
            if (forum == null)
            {
                return HttpNotFound();
            }

            return View(forum);
        }

        // GET: Forums/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Forums/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Forum forum)
        {
            if (ModelState.IsValid)
            {
                _context.Forums.Add(forum);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(forum);
        }

        // GET: Forums/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Forum forum = _context.Forums.Single(m => m.Id == id);
            if (forum == null)
            {
                return HttpNotFound();
            }
            return View(forum);
        }

        // POST: Forums/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Forum forum)
        {
            if (ModelState.IsValid)
            {
                _context.Update(forum);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(forum);
        }

        // GET: Forums/Delete/5
        [ActionName("Delete")]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Forum forum = _context.Forums.Single(m => m.Id == id);
            if (forum == null)
            {
                return HttpNotFound();
            }

            return View(forum);
        }

        // POST: Forums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            Forum forum = _context.Forums.Single(m => m.Id == id);
            _context.Forums.Remove(forum);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
