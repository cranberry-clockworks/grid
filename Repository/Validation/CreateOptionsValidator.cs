using FluentValidation;

namespace Repository.Validations;

internal class CreateOptionsValidator : AbstractValidator<CreateOptions>
{
    public CreateOptionsValidator()
    {
        RuleFor(o => o.Rows).GreaterThanOrEqualTo(0);
        RuleFor(o => o.Columns).GreaterThanOrEqualTo(0);
        RuleFor(o => o.Hash).NotEmpty();
    }
}
