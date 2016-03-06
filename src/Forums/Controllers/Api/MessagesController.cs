using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Entities;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;

namespace Forums.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/Messages")]
    [Authorize]
    public class MessagesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly MapperConfiguration _mapperConfiguration;

        public MessagesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, MapperConfiguration mapperConfiguration)
        {
            _context = context;
            _userManager = userManager;
            _mapperConfiguration = mapperConfiguration;
        }

        // GET: api/Messages/IncomingMessages
        [HttpGet]
        [Route("IncomingMessages")]

        public async Task<IActionResult> GetIncomingMessages([FromQuery] string userId)
        {
            if (userId == null)
                userId = User.GetUserId();
            else
            {
                var currentUser = await GetCurrentUserAsync();
                var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");
                if (!isAdmin)
                    return HttpUnauthorized();
            }
            return Ok(_context.Messages.Where(x=> x.RecipientId == userId));
        }

        // GET: api/Messages/IncomingMessages
        [HttpGet]
        [Route("OutgoingMessages")]

        public async Task<IActionResult> GetOutgoingMessages([FromQuery] string userId)
        {
            if (userId == null)
                userId = User.GetUserId();
            else
            {
                var currentUser = await GetCurrentUserAsync();
                var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");
                if (!isAdmin)
                    return HttpUnauthorized();
            }
            return Ok(_context.Messages.Where(x => x.SenderId == userId));
        }

        // GET: api/Messages/5
        [HttpGet("{id}", Name = "GetMessage")]
        public async Task<IActionResult> GetMessage([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            Message message = await _context.Messages.SingleAsync(m => m.Id == id);

            if (message == null)
            {
                return HttpNotFound();
            }

            return Ok(message);
        }

        // PUT: api/Messages/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMessage([FromRoute] int id, [FromBody] Message message)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            if (id != message.Id)
            {
                return HttpBadRequest();
            }

            _context.Entry(message).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MessageExists(id))
                {
                    return HttpNotFound();
                }
                else
                {
                    throw;
                }
            }

            return new HttpStatusCodeResult(StatusCodes.Status204NoContent);
        }

        // POST: api/Messages
        [HttpPost]
        public async Task<IActionResult> PostMessage([FromBody] Message message)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            _context.Messages.Add(message);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (MessageExists(message.Id))
                {
                    return new HttpStatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("GetMessage", new { id = message.Id }, message);
        }

        // DELETE: api/Messages/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessage([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            Message message = await _context.Messages.SingleAsync(m => m.Id == id);
            if (message == null)
            {
                return HttpNotFound();
            }

            var currentUserId = HttpContext.User.GetUserId();
            if (message.RecipientId == currentUserId)
                message.IsRecipientDeleted = true;
            else if (message.SenderId == currentUserId)
                message.IsSenderDeleted = true;
            else
                return HttpUnauthorized();

            await _context.SaveChangesAsync();

            return Ok(message);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool MessageExists(int id)
        {
            return _context.Messages.Count(e => e.Id == id) > 0;
        }

        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            return await _userManager.FindByIdAsync(HttpContext.User.GetUserId());
        }
    }
}