using FluentValidation;

namespace Monte.Contracts;

public interface IDateRange
{
    DateTime DateFrom { get; }
    DateTime DateTo { get; }
}

public class DateRangeValidator : AbstractValidator<IDateRange>
{
    public DateRangeValidator()
    {
        RuleFor(x => x.DateFrom)
            .LessThanOrEqualTo(x => x.DateTo);
    }
}
