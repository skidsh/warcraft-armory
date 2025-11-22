namespace WarcraftArmory.Domain.Exceptions;

/// <summary>
/// Exception thrown when an entity is invalid.
/// </summary>
public class InvalidEntityException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidEntityException"/> class.
    /// </summary>
    public InvalidEntityException()
        : base("The entity is invalid.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidEntityException"/> class.
    /// </summary>
    /// <param name="message">The error message</param>
    public InvalidEntityException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidEntityException"/> class.
    /// </summary>
    /// <param name="message">The error message</param>
    /// <param name="innerException">The inner exception</param>
    public InvalidEntityException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
