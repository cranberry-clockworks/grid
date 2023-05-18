using FluentValidation;

internal class MatrixFilesValidator: AbstractValidator<IFormFileCollection>
{   
    private static readonly IReadOnlySet<string> Names = new HashSet<string>
    {
        "First",
        "Second"
    };

    private const int OneGb = 1_073_741_824;

    public MatrixFilesValidator()
    {
        RuleFor(files => files.Count).Equal(2);
        RuleForEach(files => files)
            .ChildRules(
                file => {
                    file.RuleFor(x => x.Name).Must(name => Names.Contains(name));
                    file.RuleFor(x => x.Length).LessThanOrEqualTo(OneGb);
                }
            );
    }  
}