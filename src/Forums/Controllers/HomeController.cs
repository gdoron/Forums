using System.Linq;
using Entities;
using Forums.Extensions;
using Forums.Filters;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;

namespace Forums.Controllers
{
    [ServiceFilter(typeof (EntityFrameworkFilter))]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private ILogger _logger;

        public HomeController(ILoggerFactory iLoggerFactory, ApplicationDbContext context)
        {
            _context = context;
            _logger = iLoggerFactory.CreateLogger(nameof(HomeController));
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application TEST page.";


            //var hierarchyPosts = _context.HierarchyPosts.ToList();
            var p = _context.Posts.SingleOrDefault(x => x.Id == 1);
            var p1 = _context.Posts.SingleOrDefault(x => x.Id == 0);
            var p2 = _context.Posts.ToList();
            
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = @"Meie take a look Your page.....1232.";
            return View();
        }
        [HttpPost]
        public IActionResult Contact(string str)
        {
            var parsed = (object)str.ParseMarkdown();
            Response.Headers.Add("X-XSS-Protection", "0");
            return View(parsed);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}