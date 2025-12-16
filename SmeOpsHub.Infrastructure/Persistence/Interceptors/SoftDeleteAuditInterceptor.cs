using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using SmeOpsHub.SharedKernel;

namespace SmeOpsHub.Infrastructure.Persistence.Interceptors;

public sealed class SoftDeleteAuditInterceptor : SaveChangesInterceptor
{
    private readonly ILogger<SoftDeleteAuditInterceptor> _logger;
    private readonly ICurrentUserService _currentUser;

    public SoftDeleteAuditInterceptor(ILogger<SoftDeleteAuditInterceptor> logger, ICurrentUserService currentUser)
    {
        _logger = logger;
        _currentUser = currentUser;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        Audit(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        Audit(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void Audit(DbContext? db)
    {
        if (db is null) return;

        foreach (var entry in db.ChangeTracker.Entries()
                     .Where(e => e.State == EntityState.Modified && e.Entity is ISoftDeletable))
        {
            var isDeletedProp = entry.Property(nameof(ISoftDeletable.IsDeleted));
            if (!isDeletedProp.IsModified) continue;

            var before = (bool)isDeletedProp.OriginalValue!;
            var after = (bool)isDeletedProp.CurrentValue!;

            if (before == after) continue;

            var entityName = entry.Metadata.ClrType.Name;
            var idValue = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "Id")?.CurrentValue;

            if (after)
            {
                _logger.LogInformation("SOFT_DELETE {Entity} Id={Id} By={UserId}",
                    entityName, idValue, _currentUser.UserId);
            }
            else
            {
                _logger.LogInformation("RESTORE {Entity} Id={Id} By={UserId}",
                    entityName, idValue, _currentUser.UserId);
            }
        }
    }
}