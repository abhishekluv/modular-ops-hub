using Microsoft.AspNetCore.Mvc;
using SmeOpsHub.Modules.Hr.Application;
using SmeOpsHub.Modules.Hr.Application.Models;

namespace SmeOpsHub.Modules.Hr.Presentation.Areas.HR.Controllers;

[Area("HR")]
public class EmployeesController : Controller
{
    private readonly IEmployeeService _employees;

    public EmployeesController(IEmployeeService employees) => _employees = employees;

    public async Task<IActionResult> Index(bool showDeleted = false, CancellationToken ct = default)
    {
        var list = await _employees.GetAllAsync(includeDeleted: showDeleted, ct: ct);
        ViewBag.ShowDeleted = showDeleted;
        return View(list);
    }

    public IActionResult Create() => View(new EmployeeUpsertVm());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EmployeeUpsertVm vm, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(vm);

        try
        {
            await _employees.CreateAsync(vm, ct);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(vm);
        }
    }

    public async Task<IActionResult> Edit(Guid id, CancellationToken ct)
    {
        var vm = await _employees.GetForEditAsync(id, ct: ct);
        return vm is null ? NotFound() : View(vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, EmployeeUpsertVm vm, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(vm);

        try
        {
            var ok = await _employees.UpdateAsync(id, vm, ct);
            return ok ? RedirectToAction(nameof(Index)) : NotFound();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(vm);
        }
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SoftDelete(Guid id, CancellationToken ct)
    {
        await _employees.SoftDeleteAsync(id, ct);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Restore(Guid id, CancellationToken ct)
    {
        await _employees.RestoreAsync(id, ct);
        return RedirectToAction(nameof(Index), new { showDeleted = true });
    }
}
