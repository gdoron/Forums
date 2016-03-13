using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Entities;
using Forums.Models;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using StackExchange.Redis;

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
        private readonly ConnectionMultiplexer _redis;

        private Lazy<IMapper> Mapper
        {
            get
            {
                return new Lazy<IMapper>(()=> _mapperConfiguration.CreateMapper());
            }
        }

        public MessagesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, MapperConfiguration mapperConfiguration, ConnectionMultiplexer redis)
        {
            _context = context;
            _userManager = userManager;
            _mapperConfiguration = mapperConfiguration;
            _redis = redis;
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

            var messages = await _context.Messages.Include(x => x.Sender)
                .Where(x => x.RecipientId == userId && !x.IsRecipientDeleted)
                .OrderByDescending(x => x.SentDate)
                .ProjectTo<IncomingMessageModel>(_mapperConfiguration)
                .ToListAsync();

            return Ok(messages);
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
            var messages = await _context.Messages.Include(x => x.Sender)
                .Where(x => x.SenderId == userId && !x.IsSenderDeleted)
                .OrderByDescending(x => x.SentDate)
                .ProjectTo<OutgoingMessageModel>(_mapperConfiguration)
                .ToListAsync();

            return Ok(messages);
        }

        // GET: api/Messages/5
        [HttpGet("{id}", Name = "GetMessage")]
        [Authorize(Roles="Admin")]
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
        [Authorize(Roles = "Admin")]
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
            var currentUser = await GetCurrentUserAsync();
            message.SenderId = currentUser.Id;
            _context.Messages.Add(message);
            try
            {
                await _context.SaveChangesAsync();
                _redis.GetSubscriber().Publish($"message:{message.RecipientId}", $"New incoming message from {currentUser.UserName}");
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