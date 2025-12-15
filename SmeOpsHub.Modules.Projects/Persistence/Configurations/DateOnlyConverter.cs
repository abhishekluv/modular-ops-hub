using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace SmeOpsHub.Modules.Projects.Persistence.Configurations;

public sealed class DateOnlyConverter : ValueConverter<DateOnly?, DateTime?>
{
    public DateOnlyConverter()
        : base(
            d => d.HasValue ? d.Value.ToDateTime(TimeOnly.MinValue) : null,
            d => d.HasValue ? DateOnly.FromDateTime(d.Value) : null)
    { }
}
