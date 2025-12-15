using SmeOpsHub.SharedKernel;

namespace SmeOpsHub.Modules.Crm.Domain;

public class Company : SoftDeletableEntity<Guid>, IAggregateRoot
{
    public string Name { get; private set; } = null!;
    public string? Website { get; private set; }
    public LeadStage LeadStage { get; private set; }

    public List<Contact> Contacts { get; private set; } = new();

    private Company() { } // EF

    private Company(Guid id, string name, string? website, LeadStage leadStage) : base(id)
    {
        Name = name;
        Website = website;
        LeadStage = leadStage;
    }

    public static Company Create(string name, string? website, LeadStage leadStage)
        => new(Guid.NewGuid(), name.Trim(), website?.Trim(), leadStage);

    public void Update(string name, string? website, LeadStage leadStage)
    {
        Name = name.Trim();
        Website = website?.Trim();
        LeadStage = leadStage;
    }
}
