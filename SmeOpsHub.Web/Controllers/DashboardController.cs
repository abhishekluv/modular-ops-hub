using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmeOpsHub.Infrastructure.Persistence;
using SmeOpsHub.Modules.Crm.Domain;
using SmeOpsHub.Modules.Hr.Domain;
using SmeOpsHub.Modules.Projects.Domain;
using TaskStatus = SmeOpsHub.Modules.Projects.Domain.TaskStatus;

namespace SmeOpsHub.Web.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly AppDbContext _db;
    public DashboardController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var vm = new DashboardVm
        {
            Companies = await _db.Set<Company>().CountAsync(ct),
            Projects = await _db.Set<Project>().CountAsync(ct),
            Employees = await _db.Set<Employee>().CountAsync(ct),
            PendingLeaves = await _db.Set<LeaveRequest>().CountAsync(l => l.Status == LeaveStatus.Pending, ct),
            OpenTasks = await _db.Set<ProjectTask>().CountAsync(t => t.Status != TaskStatus.Done, ct)
        };

        return View(vm);
    }
}

public class DashboardVm
{
    public int Companies { get; set; }
    public int Projects { get; set; }
    public int Employees { get; set; }
    public int PendingLeaves { get; set; }
    public int OpenTasks { get; set; }
}
