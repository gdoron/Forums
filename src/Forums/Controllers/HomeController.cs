﻿using System.Linq;
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
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application TEST page.";


            var hierarchyPosts = _context.HierarchyPosts.ToList();
            
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