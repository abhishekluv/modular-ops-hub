using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmeOpsHub.Infrastructure.Identity;
using SmeOpsHub.SharedKernel;
using System.Linq.Expressions;

namespace SmeOpsHub.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply EF configs from all module assemblies (CRM, Projects, HR)
        var moduleAssemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.GetName().Name?.StartsWith("SmeOpsHub.Modules.", StringComparison.OrdinalIgnoreCase) == true);

        foreach (var asm in moduleAssemblies)
            modelBuilder.ApplyConfigurationsFromAssembly(asm);

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
