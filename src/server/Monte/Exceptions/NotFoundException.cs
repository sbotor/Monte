namespace Monte.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string? message = "The resource was not found.")
        : base(message)
    {
    }
}
