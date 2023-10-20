namespace Monte.Models.Exceptions;

public class RequestValidationException : Exception
{
    public IReadOnlyCollection<ErrorModel> Errors { get; set; }
    
    public RequestValidationException(IEnumerable<ErrorModel> errors)
        : base("Validation errors occured.")
    {
        Errors = errors.ToArray();
    }
}