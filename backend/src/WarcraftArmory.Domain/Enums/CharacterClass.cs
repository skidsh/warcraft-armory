namespace WarcraftArmory.Domain.Enums;

/// <summary>
/// Represents World of Warcraft character classes.
/// </summary>
/// <remarks>
/// Class IDs match Blizzard's API values. The API does not return 0 for any class;
/// Unknown (0) is used as a fallback for parsing errors or future/unrecognized classes.
/// </remarks>
public enum CharacterClass
{
    /// <summary>
    /// Unknown class (used as default/fallback, not returned by Blizzard API)
    /// </summary>
    Unknown = 0,
    Warrior = 1,
    Paladin = 2,
    Hunter = 3,
    Rogue = 4,
    Priest = 5,
    DeathKnight = 6,
    Shaman = 7,
    Mage = 8,
    Warlock = 9,
    Monk = 10,
    Druid = 11,
    DemonHunter = 12,
    Evoker = 13
}
