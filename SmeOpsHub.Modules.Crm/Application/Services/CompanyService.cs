using Microsoft.EntityFrameworkCore;
using SmeOpsHub.Infrastructure.Persistence;
using SmeOpsHub.Modules.Crm.Application.Models;
using SmeOpsHub.Modules.Crm.Domain;
using SmeOpsHub.SharedKernel;

namespace SmeOpsHub.Modules.Crm.Application.Services;

public class CompanyService : ICompanyService
{
    private readonly AppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CompanyService(AppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<CompanyListItemVm>> GetAllAsync(bool includeDeleted = false, CancellationToken ct = default)
    {
        IQueryable<Company> query = _db.Set<Company>();

        if (includeDeleted)
            query = query.IgnoreQueryFilters();

        return await query
            .OrderBy(c => c.Name)
            .Select(c => new CompanyListItemVm(
                c.Id,
                c.Name,
                c.LeadStage,
                c.Contacts.Count,
                c.IsDeleted))
            .ToListAsync(ct);
    }

    public async Task<Guid> CreateAsync(CompanyUpsertVm model, CancellationToken ct = default)
    {
        var company = Company.Create(model.Name, model.Website, model.LeadStage);
        _db.Add(company);
        await _db.SaveChangesAsync(ct);
        return company.Id;
    }

    public async Task<CompanyUpsertVm?> GetForEditAsync(Guid id, bool includeDeleted = false, CancellationToken ct = default)
    {
        IQueryable<Company> query = _db.Set<Company>();
        if (includeDeleted) query = query.IgnoreQueryFilters();

        var company = await query.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (company is null) return null;

        return new CompanyUpsertVm
        {
            Name = company.Name,
            Website = company.Website,
            LeadStage = company.LeadStage
        };
    }

    public async Task<bool> UpdateAsync(Guid id, CompanyUpsertVm model, CancellationToken ct = default)
    {
        var company = await _db.Set<Company>().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (company is null) return false;

        company.Update(model.Name, model.Website, model.LeadStage);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> SoftDeleteCompanyAsync(Guid id, CancellationToken ct = default)
    {
        var company = await _db.Set<Company>()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (company is null) return false;

        company.SoftDelete(_currentUser.UserId);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> RestoreCompanyAsync(Guid id, CancellationToken ct = default)
    {
        var company = await _db.Set<Company>()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (company is null) return false;

        company.Restore();
        await _db.SaveChangesAsync(ct);
        return true;
    }
}
