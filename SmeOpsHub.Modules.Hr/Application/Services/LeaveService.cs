using Microsoft.EntityFrameworkCore;
using SmeOpsHub.Infrastructure.Persistence;
using SmeOpsHub.Modules.Hr.Application.Models;
using SmeOpsHub.Modules.Hr.Domain;
using SmeOpsHub.SharedKernel;
using SmeOpsHub.SharedKernel.Auditing;
using System.Text.Json;

namespace SmeOpsHub.Modules.Hr.Application.Services;

public class LeaveService : ILeaveService
{
    private readonly AppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public LeaveService(AppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<LeaveRequestListItemVm>> GetAllAsync(Guid? employeeId = null, bool includeDeleted = false, CancellationToken ct = default)
    {
        IQueryable<LeaveRequest> q = _db.Set<LeaveRequest>();

        if (includeDeleted) q = q.IgnoreQueryFilters();
        if (employeeId.HasValue) q = q.Where(l => l.EmployeeId == employeeId.Value);

        return await q.AsNoTracking()
            .OrderByDescending(l => l.RequestedAt)
            .Select(l => new LeaveRequestListItemVm(
                l.Id,
                l.EmployeeId,
                l.Employee.FirstName + " " + l.Employee.LastName,
                l.Type,
                l.Status,
                l.StartDate,
                l.EndDate,
                l.Reason,
                l.IsDeleted))
            .ToListAsync(ct);
    }

    public async Task<Guid> CreateAsync(LeaveRequestCreateVm vm, CancellationToken ct = default)
    {
        ValidateLeaveDates(vm.StartDate, vm.EndDate);

        // employee must exist and be active (not deleted)
        var employeeExists = await _db.Set<Employee>().AnyAsync(e => e.Id == vm.EmployeeId, ct);
        if (!employeeExists) throw new InvalidOperationException("Employee not found.");

        // overlap check: no overlapping Pending/Approved for the same employee
        var hasOverlap = await _db.Set<LeaveRequest>()
            .AnyAsync(l =>
                l.EmployeeId == vm.EmployeeId &&
                (l.Status == LeaveStatus.Pending || l.Status == LeaveStatus.Approved) &&
                vm.StartDate <= l.EndDate &&
                vm.EndDate >= l.StartDate,
                ct);

        if (hasOverlap)
            throw new InvalidOperationException("Leave request overlaps with an existing pending/approved request.");

        var leave = LeaveRequest.Create(vm.EmployeeId, vm.Type, vm.StartDate, vm.EndDate, vm.Reason);
        _db.Add(leave);
        await _db.SaveChangesAsync(ct);
        return leave.Id;
    }

    public async Task<bool> ApproveAsync(Guid id, string? note, CancellationToken ct = default)
    {
        var leave = await _db.Set<LeaveRequest>().IgnoreQueryFilters().FirstOrDefaultAsync(l => l.Id == id, ct);
        if (leave is null) return false;

        leave.Approve(_currentUser.UserId, note);

        _db.Set<AuditEvent>().Add(new AuditEvent
        {
            Action = "LEAVE_APPROVED",
            EntityType = nameof(LeaveRequest),
            EntityId = leave.Id.ToString(),
            Module = "HR",
            Summary = $"Approved leave {leave.Id} for employee {leave.EmployeeId}",
            UserId = _currentUser.UserId,
            UserName = _currentUser.UserId,
            DataJson = JsonSerializer.Serialize(new
            {
                leave.EmployeeId,
                leave.StartDate,
                leave.EndDate,
                leave.Type
            })
        });

        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> RejectAsync(Guid id, string note, CancellationToken ct = default)
    {
        var leave = await _db.Set<LeaveRequest>().IgnoreQueryFilters().FirstOrDefaultAsync(l => l.Id == id, ct);
        if (leave is null) return false;

        leave.Reject(_currentUser.UserId, note);

        _db.Set<AuditEvent>().Add(new AuditEvent
        {
            Action = "LEAVE_REJECTED",
            EntityType = nameof(LeaveRequest),
            EntityId = leave.Id.ToString(),
            Module = "HR",
            Summary = $"Rejected leave {leave.Id} for employee {leave.EmployeeId}",
            UserId = _currentUser.UserId,
            UserName = _currentUser.UserId,
            DataJson = JsonSerializer.Serialize(new
            {
                leave.EmployeeId,
                leave.StartDate,
                leave.EndDate,
                leave.Type,
                Note = note
            })
        });

        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> SoftDeleteAsync(Guid id, CancellationToken ct = default)
    {
        var leave = await _db.Set<LeaveRequest>().IgnoreQueryFilters().FirstOrDefaultAsync(l => l.Id == id, ct);
        if (leave is null) return false;

        leave.SoftDelete(_currentUser.UserId);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> RestoreAsync(Guid id, CancellationToken ct = default)
    {
        var leave = await _db.Set<LeaveRequest>().IgnoreQueryFilters().FirstOrDefaultAsync(l => l.Id == id, ct);
        if (leave is null) return false;

        leave.Restore();
        await _db.SaveChangesAsync(ct);
        return true;
    }

    private static void ValidateLeaveDates(DateOnly start, DateOnly end)
    {
        if (start > end)
            throw new InvalidOperationException("Start date must be on or before end date.");

        var days = end.DayNumber - start.DayNumber + 1;
        if (days <= 0) throw new InvalidOperationException("Invalid leave duration.");
        if (days > 60) throw new InvalidOperationException("Leave duration too long (max 60 days).");
    }
}
