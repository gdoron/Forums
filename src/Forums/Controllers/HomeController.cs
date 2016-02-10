using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Forums.Filters;
using Forums.Models;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;

namespace Forums.Controllers
{
    [ServiceFilter(typeof(EntityFrameworkFilter))]
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
            var users = _context.Users.ToList();
            var newUser = new ApplicationUser()
                              {
                                  UserName = "Doron",
                                  Email = "doron@jifiti.com",
                                  EmailConfirmed = true
                              };
            _context.Add(newUser);

            var forums = _context.Forums.ToList();
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
            ViewData["Message"] = "Your TESTING page.....1232.";
            var newUser = new ApplicationUser() {UserName = "doron", Email = "grdoron@gmail.com"};
            _context.Users.Add(newUser);
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

    }
}
