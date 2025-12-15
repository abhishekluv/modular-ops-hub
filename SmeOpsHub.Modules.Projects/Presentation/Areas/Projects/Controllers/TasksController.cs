using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmeOpsHub.Modules.Projects.Application;
using SmeOpsHub.Modules.Projects.Application.Models;
using SmeOpsHub.SharedKernel;

namespace SmeOpsHub.Modules.Projects.Presentation.Areas.Projects.Controllers;

[Area("Projects")]
public class TasksController : Controller
{
    private readonly ITaskService _tasks;
    private readonly ICrmContactLookup _contacts;

    public TasksController(ITaskService tasks, ICrmContactLookup contacts)
    {
        _tasks = tasks;
        _contacts = contacts;
    }

    public async Task<IActionResult> Create(Guid projectId, CancellationToken ct)
    {
        await LoadContactsAsync(ct);
        ViewBag.ProjectId = projectId;
        return View(new TaskUpsertVm());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Guid projectId, TaskUpsertVm model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            await LoadContactsAsync(ct);
            ViewBag.ProjectId = projectId;
            return View(model);
        }

        await _tasks.CreateAsync(projectId, model, ct);
        return RedirectToAction("Details", "Projects", new { id = projectId });
    }

    public async Task<IActionResult> Edit(Guid id, Guid projectId, CancellationToken ct)
    {
        var vm = await _tasks.GetForEditAsync(id, includeDeleted: true, ct: ct);
        if (vm is null) return NotFound();

        await LoadContactsAsync(ct);
        ViewBag.ProjectId = projectId;
        ViewBag.TaskId = id;
        return View(vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, Guid projectId, TaskUpsertVm model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            await LoadContactsAsync(ct);
            ViewBag.ProjectId = projectId;
            ViewBag.TaskId = id;
            return View(model);
        }

        await _tasks.UpdateAsync(id, model, ct);
        return RedirectToAction("Details", "Projects", new { id = projectId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SoftDelete(Guid id, Guid projectId, CancellationToken ct)
    {
        await _tasks.SoftDeleteAsync(id, ct);
        return RedirectToAction("Details", "Projects", new { id = projectId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Restore(Guid id, Guid projectId, CancellationToken ct)
    {
        await _tasks.RestoreAsync(id, ct);
        return RedirectToAction("Details", "Projects", new { id = projectId, showDeletedTasks = true });
    }

    private async Task LoadContactsAsync(CancellationToken ct)
    {
        var contacts = await _contacts.GetActiveContactsAsync(ct);
        var items = contacts.Select(c =>
            new SelectListItem($"{c.FullName}{(string.IsNullOrWhiteSpace(c.CompanyName) ? "" : $" ({c.CompanyName})")}", c.Id.ToString()))
            .ToList();

        items.Insert(0, new SelectListItem("— Unassigned —", ""));

        ViewBag.ContactSelectList = items;
    }
}
