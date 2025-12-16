using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace SmeOpsHub.Modules.Hr.Persistence;

public sealed class DateOnlyConverter : ValueConverter<DateOnly, DateTime>
{
    public DateOnlyConverter()
        : base(
            d => d.ToDateTime(TimeOnly.MinValue),
            d => DateOnly.FromDateTime(d))
    { }
}
