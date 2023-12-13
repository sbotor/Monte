namespace Monte.Models.Exceptions;

public class ValidationException : BadRequestException
{
    public IReadOnlyDictionary<string, string[]> ErrorCodes { get; }

    public ValidationException(IReadOnlyDictionary<string, string[]> errorCodes)
    {
        ErrorCodes = errorCodes;
    }
}
