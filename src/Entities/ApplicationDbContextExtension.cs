using System.Linq;
using System.Reflection;
using Microsoft.Data.Entity;

namespace Entities
{
    public static class ApplicationDbContextExtension
    {
        public static void UseDbSetNamesAsTableNames(this DbContext dbContext, ModelBuilder modelBuilder)
        {
            var dbSets = dbContext.GetType().GetRuntimeProperties()
                .Where(p => p.PropertyType.Name == "DbSet`1")
                .Select(p => new
                                 {
                                     PropertyName = p.Name,
                                     EntityType = p.PropertyType.GenericTypeArguments.Single()
                                 })
                .ToArray();

            foreach (var type in modelBuilder.Model.GetEntityTypes())
            {
                var dbset = dbSets.SingleOrDefault(s => s.EntityType == type.ClrType);
                if (dbset != null)
                {
                    type.Relational().TableName = dbset.PropertyName;
                }
            }
        }
    }
}