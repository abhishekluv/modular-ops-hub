using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace SmeOpsHub.Modules.Projects.Persistence.Configurations;

public sealed class DateOnlyComparer : ValueComparer<DateOnly?>
{
    public DateOnlyComparer()
        : base(
            (a, b) => a == b,
            d => d.HasValue ? d.Value.GetHashCode() : 0,
            d => d)
    { }
}
