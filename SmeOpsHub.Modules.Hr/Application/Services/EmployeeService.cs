using Microsoft.EntityFrameworkCore;
using SmeOpsHub.Infrastructure.Persistence;
using SmeOpsHub.Modules.Hr.Application.Models;
using SmeOpsHub.Modules.Hr.Domain;
using SmeOpsHub.SharedKernel;

namespace SmeOpsHub.Modules.Hr.Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly AppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public EmployeeService(AppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<EmployeeListItemVm>> GetAllAsync(bool includeDeleted = false, CancellationToken ct = default)
    {
        IQueryable<Employee> q = _db.Set<Employee>();
        if (includeDeleted) q = q.IgnoreQueryFilters();

        return await q.AsNoTracking()
            .OrderBy(e => e.FirstName).ThenBy(e => e.LastName)
            .Select(e => new EmployeeListItemVm(
                e.Id,
                e.FirstName + " " + e.LastName,
                e.Email,
                e.Department,
                e.JoinDate,
                e.IsDeleted))
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<EmployeeLookupVm>> GetActiveLookupAsync(CancellationToken ct = default)
    {
        return await _db.Set<Employee>().AsNoTracking()
            .OrderBy(e => e.FirstName).ThenBy(e => e.LastName)
            .Select(e => new EmployeeLookupVm(e.Id, e.FirstName + " " + e.LastName))
            .ToListAsync(ct);
    }

    public async Task<Guid> CreateAsync(EmployeeUpsertVm vm, CancellationToken ct = default)
    {
        var employee = Employee.Create(vm.FirstName, vm.LastName, vm.Email, vm.Department, vm.JoinDate);
        _db.Add(employee);
        await _db.SaveChangesAsync(ct);
        return employee.Id;
    }

    public async Task<EmployeeUpsertVm?> GetForEditAsync(Guid id, bool includeDeleted = false, CancellationToken ct = default)
    {
        IQueryable<Employee> q = _db.Set<Employee>();
        if (includeDeleted) q = q.IgnoreQueryFilters();

        var e = await q.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (e is null) return null;

        return new EmployeeUpsertVm
        {
            FirstName = e.FirstName,
            LastName = e.LastName,
            Email = e.Email,
            Department = e.Department,
            JoinDate = e.JoinDate
        };
    }

    public async Task<bool> UpdateAsync(Guid id, EmployeeUpsertVm vm, CancellationToken ct = default)
    {
        var e = await _db.Set<Employee>().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (e is null) return false;

        e.Update(vm.FirstName, vm.LastName, vm.Email, vm.Department, vm.JoinDate);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> SoftDeleteAsync(Guid id, CancellationToken ct = default)
    {
        var e = await _db.Set<Employee>().IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (e is null) return false;

        e.SoftDelete(_currentUser.UserId);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> RestoreAsync(Guid id, CancellationToken ct = default)
    {
        var e = await _db.Set<Employee>().IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (e is null) return false;

        e.Restore();
        await _db.SaveChangesAsync(ct);
        return true;
    }
}
