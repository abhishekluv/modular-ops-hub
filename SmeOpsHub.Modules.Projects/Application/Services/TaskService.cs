using Microsoft.EntityFrameworkCore;
using SmeOpsHub.Infrastructure.Persistence;
using SmeOpsHub.Modules.Projects.Application.Models;
using SmeOpsHub.Modules.Projects.Domain;
using SmeOpsHub.SharedKernel;

namespace SmeOpsHub.Modules.Projects.Application.Services;

public class TaskService : ITaskService
{
    private readonly AppDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly ICrmContactLookup _crmContacts;

    public TaskService(AppDbContext db, ICurrentUserService currentUser, ICrmContactLookup crmContacts)
    {
        _db = db;
        _currentUser = currentUser;
        _crmContacts = crmContacts;
    }


    public async Task<IReadOnlyList<TaskListItemVm>> GetForProjectAsync(Guid projectId, bool includeDeleted = false, CancellationToken ct = default)
    {
        IQueryable<ProjectTask> q = _db.Set<ProjectTask>()
            .Where(t => t.ProjectId == projectId);

        if (includeDeleted) q = q.IgnoreQueryFilters();

        // 1) Load tasks (light projection)
        var tasks = await q.AsNoTracking()
            .OrderBy(t => t.DueDate)
            .ThenBy(t => t.Title)
            .Select(t => new
            {
                t.Id,
                t.ProjectId,
                t.Title,
                t.Status,
                t.Priority,
                t.DueDate,
                t.AssignedToContactId,
                t.IsDeleted
            })
            .ToListAsync(ct);

        // 2) Collect unique assigned contact ids
        var contactIds = tasks
            .Where(t => t.AssignedToContactId.HasValue)
            .Select(t => t.AssignedToContactId!.Value)
            .Distinct()
            .ToList();

        // 3) Lookup only those contacts (cross-module)
        var contacts = await _crmContacts.GetContactsByIdsAsync(contactIds, ct);
        var map = contacts.ToDictionary(
            c => c.Id,
            c => string.IsNullOrWhiteSpace(c.CompanyName) ? c.FullName : $"{c.FullName} ({c.CompanyName})"
        );

        // 4) Build final VM with nice display string
        return tasks.Select(t =>
        {
            var display = "—";
            if (t.AssignedToContactId.HasValue)
            {
                display = map.TryGetValue(t.AssignedToContactId.Value, out var name)
                    ? name
                    : "Unknown / deleted contact";
            }

            return new TaskListItemVm(
                t.Id,
                t.ProjectId,
                t.Title,
                t.Status,
                t.Priority,
                t.DueDate,
                t.AssignedToContactId,
                display,
                t.IsDeleted
            );
        }).ToList();
    }


    public async Task<Guid> CreateAsync(Guid projectId, TaskUpsertVm model, CancellationToken ct = default)
    {
        // ensure project exists (and is not deleted)
        var exists = await _db.Set<Project>().AnyAsync(p => p.Id == projectId, ct);
        if (!exists) throw new InvalidOperationException("Project not found.");

        var task = ProjectTask.Create(projectId, model.Title, model.Description, model.Status, model.Priority, model.DueDate, model.AssignedToContactId);
        _db.Add(task);
        await _db.SaveChangesAsync(ct);
        return task.Id;
    }

    public async Task<TaskUpsertVm?> GetForEditAsync(Guid taskId, bool includeDeleted = false, CancellationToken ct = default)
    {
        IQueryable<ProjectTask> q = _db.Set<ProjectTask>();
        if (includeDeleted) q = q.IgnoreQueryFilters();

        var task = await q.FirstOrDefaultAsync(t => t.Id == taskId, ct);
        if (task is null) return null;

        return new TaskUpsertVm
        {
            Title = task.Title,
            Description = task.Description,
            Status = task.Status,
            Priority = task.Priority,
            DueDate = task.DueDate,
            AssignedToContactId = task.AssignedToContactId
        };
    }

    public async Task<bool> UpdateAsync(Guid taskId, TaskUpsertVm model, CancellationToken ct = default)
    {
        var task = await _db.Set<ProjectTask>().FirstOrDefaultAsync(t => t.Id == taskId, ct);
        if (task is null) return false;

        task.Update(model.Title, model.Description, model.Status, model.Priority, model.DueDate, model.AssignedToContactId);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> SoftDeleteAsync(Guid taskId, CancellationToken ct = default)
    {
        var task = await _db.Set<ProjectTask>().IgnoreQueryFilters().FirstOrDefaultAsync(t => t.Id == taskId, ct);
        if (task is null) return false;

        task.SoftDelete(_currentUser.UserId);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> RestoreAsync(Guid taskId, CancellationToken ct = default)
    {
        var task = await _db.Set<ProjectTask>().IgnoreQueryFilters().FirstOrDefaultAsync(t => t.Id == taskId, ct);
        if (task is null) return false;

        task.Restore();
        await _db.SaveChangesAsync(ct);
        return true;
    }
}
