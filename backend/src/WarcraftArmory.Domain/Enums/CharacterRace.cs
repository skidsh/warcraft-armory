namespace WarcraftArmory.Domain.Enums;

/// <summary>
/// Represents World of Warcraft character races.
/// </summary>
/// <remarks>
/// Race IDs match Blizzard's API values. The API does not return 0 for any race;
/// Unknown (0) is used as a fallback for parsing errors or future/unrecognized races.
/// </remarks>
public enum CharacterRace
{
    /// <summary>
    /// Unknown race (used as default/fallback, not returned by Blizzard API)
    /// </summary>
    Unknown = 0,
    Human = 1,
    Orc = 2,
    Dwarf = 3,
    NightElf = 4,
    Undead = 5,
    Tauren = 6,
    Gnome = 7,
    Troll = 8,
    Goblin = 9,
    BloodElf = 10,
    Draenei = 11,
    Worgen = 22,
    Pandaren = 24,
    VoidElf = 29,
    LightforgedDraenei = 30,
    HighmountainTauren = 28,
    Nightborne = 27,
    MagharOrc = 36,
    DarkIronDwarf = 34,
    KulTiran = 32,
    ZandalariTroll = 31,
    Mechagnome = 37,
    Vulpera = 35,
    Dracthyr = 52,
    EarthenDwarf = 84
}
