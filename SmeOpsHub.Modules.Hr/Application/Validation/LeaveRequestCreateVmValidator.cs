using FluentValidation;
using SmeOpsHub.Modules.Hr.Application.Models;

namespace SmeOpsHub.Modules.Hr.Application.Validation
{
    public class LeaveRequestCreateVmValidator : AbstractValidator<LeaveRequestCreateVm>
    {
        public LeaveRequestCreateVmValidator()
        {
            RuleFor(x => x.EmployeeId).NotEmpty();
            RuleFor(x => x.StartDate).NotEmpty();
            RuleFor(x => x.EndDate).NotEmpty();

            RuleFor(x => x)
                .Must(x => x.StartDate <= x.EndDate)
                .WithMessage("Start date must be on or before end date.");

            RuleFor(x => x)
                .Must(x =>
                {
                    var days = x.EndDate.DayNumber - x.StartDate.DayNumber + 1;
                    return days is >= 1 and <= 60;
                })
                .WithMessage("Leave duration must be between 1 and 60 days.");
        }
    }
}
