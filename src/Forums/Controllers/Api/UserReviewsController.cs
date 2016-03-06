using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Entities;
using Forums.Models;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;

namespace Forums.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/UserReviews")]
    public class UserReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly MapperConfiguration _mapperConfiguration;

        public UserReviewsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, MapperConfiguration mapperConfiguration)
        {
            _context = context;
            _userManager = userManager;
            _mapperConfiguration = mapperConfiguration;
        }

        // GET: api/UserReviews
        [HttpGet]
        [Route("GetUserReviewsForUser")]
        public async Task<List<UserReviewListModel>> GetUserReviewsForUser([FromQuery] string userId)
        {
            var reviews = await _context.UserReviews.Where(x => x.ToUserId == userId && !x.IsDeleted)
                .Include(x => x.FromUser).ToListAsync();
            var models = _mapperConfiguration.CreateMapper().Map<List<UserReview>, List<UserReviewListModel>>(reviews);

            return models;
        }

        // GET: api/UserReviews/5
        [HttpGet("{id}", Name = "GetUserReview")]
        public async Task<IActionResult> GetUserReview([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            UserReview userReview = await _context.UserReviews.SingleAsync(m => m.Id == id);

            if (userReview == null)
            {
                return HttpNotFound();
            }

            return Ok(userReview);
        }

        // PUT: api/UserReviews/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserReview([FromRoute] int id, [FromBody] EditUserReviewModel model)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            if (id != model.Id)
            {
                return HttpBadRequest();
            }

            var userReview = await _context.UserReviews.SingleOrDefaultAsync();
            if (userReview == null)
                    return HttpNotFound();
            
            userReview.UpdateDate = DateTime.UtcNow;
            userReview.Review = model.Review;
            userReview.VoteType = model.VoteType;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserReviewExists(id))
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

        // POST: api/UserReviews
        [HttpPost]
        public async Task<IActionResult> PostUserReview([FromBody] CreateUserReviewModel model)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var userReview = _mapperConfiguration.CreateMapper().Map<CreateUserReviewModel, UserReview>(model);
            //var currentUser = await GetCurrentUserAsync();
            userReview.FromUserId = HttpContext.User.GetUserId();
            _context.UserReviews.Add(userReview);
            await _context.SaveChangesAsync();

            return CreatedAtRoute("GetUserReview", new {id = userReview.Id}, userReview);
        }

        // DELETE: api/UserReviews/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserReview([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            UserReview userReview = await _context.UserReviews.SingleAsync(m => m.Id == id);
            if (userReview == null)
            {
                return HttpNotFound();
            }

            userReview.IsDeleted = true;
            await _context.SaveChangesAsync();

            return Ok(userReview);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UserReviewExists(int id)
        {
            return _context.UserReviews.Any(e => e.Id == id);
        }

        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            return await _userManager.FindByIdAsync(HttpContext.User.GetUserId());
        }
    }
}