using WarcraftArmory.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace WarcraftArmory.Domain.ValueObjects;

/// <summary>
/// Represents a validated realm slug (URL-safe realm name) value object.
/// </summary>
public sealed partial record RealmSlug
{
    private const int MinLength = 2;
    private const int MaxLength = 50;

    [GeneratedRegex(@"^[a-z0-9\-]+$", RegexOptions.Compiled)]
    private static partial Regex SlugPattern();

    /// <summary>
    /// Gets the realm slug value.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RealmSlug"/> class.
    /// </summary>
    /// <param name="value">The realm slug</param>
    /// <exception cref="InvalidEntityException">Thrown when the slug is invalid</exception>
    public RealmSlug(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidEntityException("Realm slug cannot be null or empty.");
        }

        var normalized = value.ToLowerInvariant();

        if (normalized.Length < MinLength || normalized.Length > MaxLength)
        {
            throw new InvalidEntityException(
                $"Realm slug must be between {MinLength} and {MaxLength} characters.");
        }

        // Slugs should only contain lowercase letters, numbers, and hyphens
        if (!SlugPattern().IsMatch(normalized))
        {
            throw new InvalidEntityException(
                "Realm slug can only contain lowercase letters, numbers, and hyphens.");
        }

        Value = normalized;
    }

    /// <summary>
    /// Implicitly converts a string to a RealmSlug.
    /// </summary>
    public static implicit operator string(RealmSlug slug) => slug.Value;

    /// <summary>
    /// Explicitly converts a string to a RealmSlug.
    /// </summary>
    public static explicit operator RealmSlug(string value) => new(value);

    /// <summary>
    /// Returns the realm slug as a string.
    /// </summary>
    public override string ToString() => Value;
}
