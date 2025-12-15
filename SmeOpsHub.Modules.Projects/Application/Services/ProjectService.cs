using Microsoft.EntityFrameworkCore;
using SmeOpsHub.Infrastructure.Persistence;
using SmeOpsHub.Modules.Projects.Application.Models;
using SmeOpsHub.Modules.Projects.Domain;
using SmeOpsHub.SharedKernel;

namespace SmeOpsHub.Modules.Projects.Application.Services;

public class ProjectService : IProjectService
{
    private readonly AppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public ProjectService(AppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<ProjectListItemVm>> GetAllAsync(bool includeDeleted = false, CancellationToken ct = default)
    {
        IQueryable<Project> q = _db.Set<Project>();

        if (includeDeleted) q = q.IgnoreQueryFilters();

        return await q
            .OrderBy(p => p.Name)
            .Select(p => new ProjectListItemVm(p.Id, p.Name, p.Status, p.Tasks.Count, p.IsDeleted))
            .ToListAsync(ct);
    }

    public async Task<Guid> CreateAsync(ProjectUpsertVm model, CancellationToken ct = default)
    {
        var project = Project.Create(model.Name, model.Description, model.Status, model.StartDate, model.EndDate);
        _db.Add(project);
        await _db.SaveChangesAsync(ct);
        return project.Id;
    }

    public async Task<ProjectUpsertVm?> GetForEditAsync(Guid id, bool includeDeleted = false, CancellationToken ct = default)
    {
        IQueryable<Project> q = _db.Set<Project>();
        if (includeDeleted) q = q.IgnoreQueryFilters();

        var project = await q.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (project is null) return null;

        return new ProjectUpsertVm
        {
            Name = project.Name,
            Description = project.Description,
            Status = project.Status,
            StartDate = project.StartDate,
            EndDate = project.EndDate
        };
    }

    public async Task<bool> UpdateAsync(Guid id, ProjectUpsertVm model, CancellationToken ct = default)
    {
        var project = await _db.Set<Project>().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (project is null) return false;

        project.Update(model.Name, model.Description, model.Status, model.StartDate, model.EndDate);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> SoftDeleteAsync(Guid id, CancellationToken ct = default)
    {
        var project = await _db.Set<Project>().IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (project is null) return false;

        project.SoftDelete(_currentUser.UserId);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> RestoreAsync(Guid id, CancellationToken ct = default)
    {
        var project = await _db.Set<Project>().IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (project is null) return false;

        project.Restore();
        await _db.SaveChangesAsync(ct);
        return true;
    }
}
