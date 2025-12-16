using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace SmeOpsHub.Modules.Hr.Persistence;

public sealed class DateOnlyComparer : ValueComparer<DateOnly>
{
    public DateOnlyComparer()
        : base(
            (a, b) => a == b,
            d => d.GetHashCode(),
            d => d)
    { }
}
