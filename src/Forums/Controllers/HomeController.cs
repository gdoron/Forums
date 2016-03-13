using System.Linq;
using System.Threading.Tasks;
using Entities;
using Forums.Extensions;
using Forums.Filters;
using Forums.ViewModels.Home;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
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

            return View();
        }

        public IActionResult Contact()
        {
            var str = @"<script>alert('xss')</script><div onload=""alert('xss')"""
    + @"style=""background-color: aqua"">Test<img src=""https://lh5.googleusercontent.com/-drrRi1dWOQQ/AAAAAAAAAAI/AAAAAAAAAAA/AMW9IgcL7q_lfB00a-OlXFlFZeUYTGjqSg/s96-c-mo/photo.jpg"""
    + @"style=""background-image: url(javascript:alert('xss')); margin: 10px""></div>";
            var model = new ContactViewModel
                            {
                                OriginalStr = str,
                                Str = str.ParseMarkdown()
                            };
            ViewData["Message"] = @"CommonMark and Anti XSS Demo";
            return View(model);
        }
        [HttpPost]
        public IActionResult Contact(ContactViewModel model)
        {
            model.OriginalStr = model.Str;
            model.Str = model.Str.ParseMarkdown();
            Response.Headers.Add("X-XSS-Protection", "0");

            return View(model);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}