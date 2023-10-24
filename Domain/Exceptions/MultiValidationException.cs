namespace Domain.Exceptions;

public class MultiValidationException : Exception
{
    public List<string> ValidationErrors { get; }

    public MultiValidationException(string validationError)
        : base(validationError)
    {
        ValidationErrors = new List<string> { validationError };
    }

    public MultiValidationException(List<string> validationErrors)
        : base(string.Join(Environment.NewLine, validationErrors))
    {
        ValidationErrors = validationErrors ?? throw new ArgumentNullException(nameof(validationErrors));
    }
}