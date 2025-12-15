using SmeOpsHub.Modules.Crm.Application.Models;

namespace SmeOpsHub.Modules.Crm.Application;

public interface ICompanyService
{
    Task<IReadOnlyList<CompanyListItemVm>> GetAllAsync(bool includeDeleted = false, CancellationToken ct = default);
    Task<Guid> CreateAsync(CompanyUpsertVm model, CancellationToken ct = default);
    Task<CompanyUpsertVm?> GetForEditAsync(Guid id, bool includeDeleted = false, CancellationToken ct = default);
    Task<bool> UpdateAsync(Guid id, CompanyUpsertVm model, CancellationToken ct = default);

    Task<bool> SoftDeleteCompanyAsync(Guid id, CancellationToken ct = default);
    Task<bool> RestoreCompanyAsync(Guid id, CancellationToken ct = default);
}
