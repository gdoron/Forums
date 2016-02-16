using System.Linq;
using Forums.Filters;
using Forums.Models;
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
            //var postsHie = _context.HierarchyPosts.ToList();

            var newUser = new ApplicationUser
                              {
                                  UserName = "Doron",
                                  Email = "doron@jifiti.com",
                                  EmailConfirmed = true
                              };

            _context.Add(newUser);
            newUser = new ApplicationUser
                          {
                              UserName = "Moshe",
                              Email = "moshe@j123ifiti.com",
                              EmailConfirmed = true
                          };
            _context.Add(newUser);

            newUser = new ApplicationUser
                          {
                              UserName = "David",
                              Email = "David123@aaajifiti.com",
                              EmailConfirmed = true
                          };
            _context.Add(newUser);

            //var forums = _context.Forums.ToList();

            var foo = _context.Posts.Single(x => x.Text == "Root level");
            var thirdPost = _context.Posts.Single(x => x.Text == "third post");

            var posts = _context.Posts.ToList();

            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application TEST page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Meie take a look Your page.....1232.";
            var newUser = new ApplicationUser {UserName = "doron", Email = "grdoron@gmail.com"};
            _context.Users.Add(newUser);
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}