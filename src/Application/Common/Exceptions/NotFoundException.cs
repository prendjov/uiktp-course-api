namespace Application.Common.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException()
        : base()
    {
    }

    public NotFoundException(string message)
        : base(message)
    {
    }

    public NotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public NotFoundException(string name, object key)
        : base($"Entity \"{name}\" ({key}) was not found.")
    {
    }

    public static NotFoundException New<T>(object key) => new(typeof(T).Name, key);

    public static NotFoundException New<T>() => new($"Entity \"{typeof(T).Name}\" was not found.");
}
