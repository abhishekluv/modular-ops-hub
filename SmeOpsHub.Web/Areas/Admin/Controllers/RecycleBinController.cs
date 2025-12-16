using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmeOpsHub.Infrastructure.Persistence;
using SmeOpsHub.Modules.Crm.Domain;
using SmeOpsHub.Modules.Hr.Domain;
using SmeOpsHub.Modules.Projects.Domain;
using SmeOpsHub.SharedKernel.Security;

namespace SmeOpsHub.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Manager}")]
public class RecycleBinController : Controller
{
    private readonly AppDbContext _db;

    public RecycleBinController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Companies(CancellationToken ct)
    {
        var items = await _db.Set<Company>()
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(x => x.IsDeleted)
            .OrderByDescending(x => x.DeletedAt)
            .Select(x => new RecycleItemVm(x.Id.ToString(), x.Name, x.DeletedAt, x.DeletedBy))
            .ToListAsync(ct);

        return View(items);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> RestoreCompany(Guid id, CancellationToken ct)
    {
        var entity = await _db.Set<Company>().IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return NotFound();

        entity.Restore();
        await _db.SaveChangesAsync(ct);

        return RedirectToAction(nameof(Companies));
    }

    public async Task<IActionResult> Projects(CancellationToken ct)
    {
        var items = await _db.Set<Project>()
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(x => x.IsDeleted)
            .OrderByDescending(x => x.DeletedAt)
            .Select(x => new RecycleItemVm(x.Id.ToString(), x.Name, x.DeletedAt, x.DeletedBy))
            .ToListAsync(ct);

        return View(items);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> RestoreProject(Guid id, CancellationToken ct)
    {
        var entity = await _db.Set<Project>().IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return NotFound();

        entity.Restore();
        await _db.SaveChangesAsync(ct);

        return RedirectToAction(nameof(Projects));
    }

    public async Task<IActionResult> Employees(CancellationToken ct)
    {
        var items = await _db.Set<Employee>()
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(x => x.IsDeleted)
            .OrderByDescending(x => x.DeletedAt)
            .Select(x => new RecycleItemVm(
                x.Id.ToString(),
                (x.FirstName + " " + x.LastName),
                x.DeletedAt,
                x.DeletedBy))
            .ToListAsync(ct);

        return View(items);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> RestoreEmployee(Guid id, CancellationToken ct)
    {
        var entity = await _db.Set<Employee>().IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return NotFound();

        entity.Restore();
        await _db.SaveChangesAsync(ct);

        return RedirectToAction(nameof(Employees));
    }
}

public record RecycleItemVm(string Id, string Title, DateTimeOffset? DeletedAt, string? DeletedBy);
