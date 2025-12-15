using Microsoft.AspNetCore.Mvc;
using SmeOpsHub.Modules.Crm.Application;
using SmeOpsHub.Modules.Crm.Application.Models;

namespace SmeOpsHub.Modules.Crm.Presentation.Areas.CRM.Controllers;

[Area("CRM")]
public class CompaniesController : Controller
{
    private readonly ICompanyService _companies;

    public CompaniesController(ICompanyService companies)
    {
        _companies = companies;
    }

    public async Task<IActionResult> Index(bool showDeleted = false, CancellationToken ct = default)
    {
        var data = await _companies.GetAllAsync(includeDeleted: showDeleted, ct: ct);
        ViewBag.ShowDeleted = showDeleted;
        return View(data);
    }

    public IActionResult Create() => View(new CompanyUpsertVm());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CompanyUpsertVm model, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(model);

        await _companies.CreateAsync(model, ct);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(Guid id, CancellationToken ct)
    {
        var vm = await _companies.GetForEditAsync(id, ct: ct);
        return vm is null ? NotFound() : View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, CompanyUpsertVm model, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(model);

        var ok = await _companies.UpdateAsync(id, model, ct);
        return ok ? RedirectToAction(nameof(Index)) : NotFound();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SoftDelete(Guid id, CancellationToken ct)
    {
        await _companies.SoftDeleteCompanyAsync(id, ct);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Restore(Guid id, CancellationToken ct)
    {
        await _companies.RestoreCompanyAsync(id, ct);
        return RedirectToAction(nameof(Index), new { showDeleted = true });
    }
}
