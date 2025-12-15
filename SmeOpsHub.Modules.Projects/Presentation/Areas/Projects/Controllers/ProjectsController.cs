using Microsoft.AspNetCore.Mvc;
using SmeOpsHub.Modules.Projects.Application;
using SmeOpsHub.Modules.Projects.Application.Models;

namespace SmeOpsHub.Modules.Projects.Presentation.Areas.Projects.Controllers;

[Area("Projects")]
public class ProjectsController : Controller
{
    private readonly IProjectService _projects;
    private readonly ITaskService _tasks;

    public ProjectsController(IProjectService projects, ITaskService tasks)
    {
        _projects = projects;
        _tasks = tasks;
    }

    public async Task<IActionResult> Index(bool showDeleted = false, CancellationToken ct = default)
    {
        var list = await _projects.GetAllAsync(includeDeleted: showDeleted, ct: ct);
        ViewBag.ShowDeleted = showDeleted;
        return View(list);
    }

    public IActionResult Create() => View(new ProjectUpsertVm());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProjectUpsertVm model, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(model);

        var id = await _projects.CreateAsync(model, ct);
        return RedirectToAction(nameof(Details), new { id });
    }

    public async Task<IActionResult> Edit(Guid id, CancellationToken ct)
    {
        var vm = await _projects.GetForEditAsync(id, ct: ct);
        return vm is null ? NotFound() : View(vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, ProjectUpsertVm model, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(model);

        var ok = await _projects.UpdateAsync(id, model, ct);
        return ok ? RedirectToAction(nameof(Details), new { id }) : NotFound();
    }

    public async Task<IActionResult> Details(Guid id, bool showDeletedTasks = false, CancellationToken ct = default)
    {
        // minimal details page: project + tasks list
        var project = await _projects.GetForEditAsync(id, includeDeleted: true, ct: ct);
        if (project is null) return NotFound();

        var tasks = await _tasks.GetForProjectAsync(id, includeDeleted: showDeletedTasks, ct: ct);

        ViewBag.ProjectId = id;
        ViewBag.ProjectName = project.Name;
        ViewBag.ShowDeletedTasks = showDeletedTasks;

        return View(tasks);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SoftDelete(Guid id, CancellationToken ct)
    {
        await _projects.SoftDeleteAsync(id, ct);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Restore(Guid id, CancellationToken ct)
    {
        await _projects.RestoreAsync(id, ct);
        return RedirectToAction(nameof(Index), new { showDeleted = true });
    }
}
