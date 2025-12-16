using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SmeOpsHub.SharedKernel;
using SmeOpsHub.SharedKernel.Auditing;

namespace SmeOpsHub.Infrastructure.Persistence.Interceptors;

public sealed class SoftDeleteAuditInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUser;

    public SoftDeleteAuditInterceptor(ICurrentUserService currentUser)
        => _currentUser = currentUser;

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        AddAuditEvents(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        AddAuditEvents(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void AddAuditEvents(DbContext? db)
    {
        if (db is null) return;

        // Avoid auditing audit rows
        if (db.ChangeTracker.Entries().Any(e => e.Entity is AuditEvent))
            return;

        foreach (var entry in db.ChangeTracker.Entries()
                     .Where(e => e.State == EntityState.Modified && e.Entity is ISoftDeletable))
        {
            var isDeletedProp = entry.Property(nameof(ISoftDeletable.IsDeleted));
            if (!isDeletedProp.IsModified) continue;

            var before = (bool)(isDeletedProp.OriginalValue ?? false);
            var after = (bool)(isDeletedProp.CurrentValue ?? false);
            if (before == after) continue;

            var entityType = entry.Entity.GetType().Name;
            var id = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "Id")?.CurrentValue?.ToString() ?? "?";

            var action = after ? "SOFT_DELETE" : "RESTORE";

            db.Set<AuditEvent>().Add(new AuditEvent
            {
                Action = action,
                EntityType = entityType,
                EntityId = id,
                Summary = $"{action} {entityType}({id})",
                UserId = _currentUser.UserId,
                UserName = _currentUser.UserName,
                TraceId = _currentUser.TraceId
            });
        }
    }
}
