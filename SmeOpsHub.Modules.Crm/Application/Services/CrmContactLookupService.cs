using Microsoft.EntityFrameworkCore;
using SmeOpsHub.Infrastructure.Persistence;
using SmeOpsHub.Modules.Crm.Domain;
using SmeOpsHub.SharedKernel;

namespace SmeOpsHub.Modules.Crm.Application.Services;

public class CrmContactLookupService : ICrmContactLookup
{
    private readonly AppDbContext _db;

    public CrmContactLookupService(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<ContactLookupItem>> GetActiveContactsAsync(CancellationToken ct = default)
    {
        return await _db.Set<Contact>()
            .AsNoTracking()
            .OrderBy(c => c.FullName)
            .Select(c => new ContactLookupItem(c.Id, c.FullName, c.Company.Name))
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<ContactLookupItem>> GetContactsByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default)
    {
        var idList = ids.Distinct().ToList();

        if(idList.Count == 0) return Array.Empty<ContactLookupItem>();

        return await _db.Set<Contact>()
            .AsNoTracking()
            .Where(c => idList.Contains(c.Id))
            .Select(c => new ContactLookupItem(c.Id, c.FullName, c.Company.Name))
            .ToListAsync(ct);
    }
}
