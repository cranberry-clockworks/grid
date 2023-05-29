using FluentValidation;
using Repository.Models;

namespace Repository.Validations;

internal class MatrixCreationOptionsValidator : AbstractValidator<MatrixCreationOptions>
{
    public MatrixCreationOptionsValidator()
    {
        RuleFor(o => o.Rows).GreaterThanOrEqualTo(0);
        RuleFor(o => o.Columns).GreaterThanOrEqualTo(0);
        RuleFor(o => o.Hash).NotEmpty();
    }
}
