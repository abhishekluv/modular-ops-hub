using Microsoft.EntityFrameworkCore;
using SmeOpsHub.SharedKernel;
using System.Linq.Expressions;

namespace SmeOpsHub.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ApplySoftDeleteQueryFilters(modelBuilder);
    }

    private static void ApplySoftDeleteQueryFilters(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            // Only apply filter to ISoftDeletable entities
            if (!typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
                continue;

            var parameter = Expression.Parameter(entityType.ClrType, "e");

            // e => !e.IsDeleted
            var isDeletedProperty = Expression.Property(
                parameter,
                nameof(ISoftDeletable.IsDeleted));

            var isNotDeleted = Expression.Equal(
                isDeletedProperty,
                Expression.Constant(false));

            var lambda = Expression.Lambda(isNotDeleted, parameter);

            modelBuilder
                .Entity(entityType.ClrType)
                .HasQueryFilter(lambda);
        }
    }
}
