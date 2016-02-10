using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Forums.Models;
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

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            var x = 2;
            base.OnResultExecuting(context);
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var x = 2;
            base.OnActionExecuting(context);
        }

        public override void OnResultExecuted(ResultExecutedContext context)
        {
            var x = 2;
            base.OnResultExecuted(context);
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
