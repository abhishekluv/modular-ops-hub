using SmeOpsHub.SharedKernel;
using System.Diagnostics;

namespace SmeOpsHub.Modules.Crm.Domain;

public class Contact : SoftDeletableEntity<Guid>
{
    public Guid CompanyId { get; private set; }
    public Company Company { get; private set; } = null!;

    public string FullName { get; private set; } = null!;
    public string? Email { get; private set; }
    public string? Phone { get; private set; }

    public List<Activity> Activities { get; private set; } = new();

    private Contact() { } // EF

    private Contact(Guid id, Guid companyId, string fullName, string? email, string? phone) : base(id)
    {
        CompanyId = companyId;
        FullName = fullName.Trim();
        Email = email?.Trim();
        Phone = phone?.Trim();
    }

    public static Contact Create(Guid companyId, string fullName, string? email, string? phone)
        => new(Guid.NewGuid(), companyId, fullName, email, phone);

    public void Update(string fullName, string? email, string? phone)
    {
        FullName = fullName.Trim();
        Email = email?.Trim();
        Phone = phone?.Trim();
    }
}
