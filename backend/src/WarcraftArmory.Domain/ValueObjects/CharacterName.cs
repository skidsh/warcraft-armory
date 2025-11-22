using WarcraftArmory.Domain.Exceptions;

namespace WarcraftArmory.Domain.ValueObjects;

/// <summary>
/// Represents a validated character name value object.
/// </summary>
public sealed record CharacterName
{
    private const int MinLength = 2;
    private const int MaxLength = 12;

    /// <summary>
    /// Gets the character name value.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CharacterName"/> class.
    /// </summary>
    /// <param name="value">The character name</param>
    /// <exception cref="InvalidEntityException">Thrown when the name is invalid</exception>
    public CharacterName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidEntityException("Character name cannot be null or empty.");
        }

        if (value.Length < MinLength || value.Length > MaxLength)
        {
            throw new InvalidEntityException(
                $"Character name must be between {MinLength} and {MaxLength} characters.");
        }

        // Validate Unicode letters (supports accented characters like Ñ, é, ü in non-English realms)
        // Blizzard allows letters from any Unicode category (Latin, Cyrillic, etc.)
        if (!value.All(c => char.IsLetter(c) || char.GetUnicodeCategory(c) == System.Globalization.UnicodeCategory.NonSpacingMark))
        {
            throw new InvalidEntityException(
                "Character name can only contain letters (including accented characters).");
        }

        Value = value;
    }

    /// <summary>
    /// Implicitly converts a string to a CharacterName.
    /// </summary>
    public static implicit operator string(CharacterName name) => name.Value;

    /// <summary>
    /// Explicitly converts a string to a CharacterName.
    /// </summary>
    public static explicit operator CharacterName(string value) => new(value);

    /// <summary>
    /// Returns the character name as a string.
    /// </summary>
    public override string ToString() => Value;
}
