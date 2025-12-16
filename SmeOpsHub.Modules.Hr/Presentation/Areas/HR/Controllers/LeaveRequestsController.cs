using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmeOpsHub.Modules.Hr.Application;
using SmeOpsHub.Modules.Hr.Application.Models;

namespace SmeOpsHub.Modules.Hr.Presentation.Areas.HR.Controllers;

[Area("HR")]
public class LeaveRequestsController : Controller
{
    private readonly ILeaveService _leaves;
    private readonly IEmployeeService _employees;

    public LeaveRequestsController(ILeaveService leaves, IEmployeeService employees)
    {
        _leaves = leaves;
        _employees = employees;
    }

    public async Task<IActionResult> Index(Guid? employeeId = null, bool showDeleted = false, CancellationToken ct = default)
    {
        var list = await _leaves.GetAllAsync(employeeId, includeDeleted: showDeleted, ct: ct);
        ViewBag.EmployeeId = employeeId;
        ViewBag.ShowDeleted = showDeleted;

        await LoadEmployeeDropdown(ct, employeeId);
        return View(list);
    }

    public async Task<IActionResult> Create(Guid? employeeId = null, CancellationToken ct = default)
    {
        await LoadEmployeeDropdown(ct, employeeId);
        return View(new LeaveRequestCreateVm { EmployeeId = employeeId ?? Guid.Empty });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(LeaveRequestCreateVm vm, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            await LoadEmployeeDropdown(ct, vm.EmployeeId);
            return View(vm);
        }

        try
        {
            await _leaves.CreateAsync(vm, ct);
            return RedirectToAction(nameof(Index), new { employeeId = vm.EmployeeId });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await LoadEmployeeDropdown(ct, vm.EmployeeId);
            return View(vm);
        }
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(Guid id, Guid? employeeId, CancellationToken ct)
    {
        await _leaves.ApproveAsync(id, note: null, ct: ct);
        return RedirectToAction(nameof(Index), new { employeeId });
    }

    public IActionResult Reject(Guid id, Guid? employeeId)
        => View(new LeaveRejectVm { Id = id, ReturnEmployeeId = employeeId });

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(LeaveRejectVm vm, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(vm);

        await _leaves.RejectAsync(vm.Id, vm.Note, ct);
        return RedirectToAction(nameof(Index), new { employeeId = vm.ReturnEmployeeId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SoftDelete(Guid id, Guid? employeeId, CancellationToken ct)
    {
        await _leaves.SoftDeleteAsync(id, ct);
        return RedirectToAction(nameof(Index), new { employeeId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Restore(Guid id, Guid? employeeId, CancellationToken ct)
    {
        await _leaves.RestoreAsync(id, ct);
        return RedirectToAction(nameof(Index), new { employeeId, showDeleted = true });
    }

    private async Task LoadEmployeeDropdown(CancellationToken ct, Guid? selectedEmployeeId)
    {
        var employees = await _employees.GetActiveLookupAsync(ct);
        var items = employees
            .Select(e => new SelectListItem(e.FullName, e.Id.ToString(), selectedEmployeeId == e.Id))
            .ToList();

        items.Insert(0, new SelectListItem("— All Employees —", "", !selectedEmployeeId.HasValue));

        ViewBag.EmployeeSelectList = items;
    }
}
