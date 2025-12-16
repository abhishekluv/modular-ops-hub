using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmeOpsHub.Infrastructure.Persistence;
using SmeOpsHub.SharedKernel.Auditing;
using SmeOpsHub.SharedKernel.Security;

namespace SmeOpsHub.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Manager}")]
public class AuditController : Controller
{
    private readonly AppDbContext _db;
    public AuditController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index(string? actionFilter = null, string? entityType = null, string? search = null, CancellationToken ct = default)
    {
        IQueryable<AuditEvent> q = _db.Set<AuditEvent>().AsNoTracking();

        if (!string.IsNullOrWhiteSpace(actionFilter))
            q = q.Where(x => x.Action == actionFilter);

        if (!string.IsNullOrWhiteSpace(entityType))
            q = q.Where(x => x.EntityType == entityType);

        if (!string.IsNullOrWhiteSpace(search))
            q = q.Where(x =>
                x.Summary!.Contains(search) ||
                x.EntityId.Contains(search) ||
                (x.UserName != null && x.UserName.Contains(search)));

        var items = await q.OrderByDescending(x => x.OccurredAtUtc)
            .Take(200)
            .ToListAsync(ct);

        ViewBag.ActionFilter = actionFilter;
        ViewBag.EntityType = entityType;
        ViewBag.Search = search;

        return View(items);
    }

    public async Task<IActionResult> Details(Guid id, CancellationToken ct)
    {
        var ev = await _db.Set<AuditEvent>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        return ev is null ? NotFound() : View(ev);
    }
}
