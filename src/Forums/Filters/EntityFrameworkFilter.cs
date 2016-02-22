using Entities;
using Microsoft.AspNet.Mvc.Filters;

namespace Forums.Filters
{
    public class EntityFrameworkFilter : ActionFilterAttribute
    {
        private readonly ApplicationDbContext _context;

        public EntityFrameworkFilter( ApplicationDbContext context)
        {
            _context = context;
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception != null)
            {
                // Rollback
            }
            else
            {
                _context.SaveChangesAsync();
            }
        }
    }
}
