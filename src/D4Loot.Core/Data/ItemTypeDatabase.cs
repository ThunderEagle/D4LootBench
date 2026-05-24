namespace D4Loot.Core.Data;

public sealed record ItemTypeEntry(string Name, uint Hash, string InternalName);

/// <summary>
/// Item type hash IDs sourced from fnuecke/diablo4-loot-filter-viewer names.json (datamined)
/// and cross-referenced against Upsilon72/d4-filter-generator Season 13 exports.
/// IDs are FIXED32 (little-endian) in the protobuf binary.
/// </summary>
public static class ItemTypeDatabase
{
    // ── Weapons ─────────────────────────────────────────────────────────────

    public static IReadOnlyList<ItemTypeEntry> Weapons { get; } =
    [
        new("Axe",               0x0006D151, "Axe"),
        new("Two-Handed Axe",    0x0006D152, "Axe2H"),
        new("Mace",              0x0006D13A, "Mace"),
        new("Two-Handed Mace",   0x0006D144, "Mace2H"),
        new("Sword",             0x0006D14C, "Sword"),
        new("Two-Handed Sword",  0x0006D14F, "Sword2H"),
        new("Dagger",            0x0006D159, "Dagger"),
        new("Polearm",           0x0006D15D, "Polearm"),
        new("Scythe",            0x0006D154, "Scythe"),
        new("Two-Handed Scythe", 0x0006D155, "Scythe2H"),
        new("Staff",             0x0006D153, "Staff"),
        new("Wand",              0x0006D163, "Wand"),
        new("Focus",             0x0006D16A, "Focus"),
        new("Bow",               0x0006D167, "Bow"),
        new("Crossbow",          0x0006D168, "Crossbow"),
        new("Two-Handed Crossbow", 0x0006D169, "Crossbow2H"),
        new("Totem",             0x0006D16B, "OffHandTotem")
    ];

    // ── Armor ────────────────────────────────────────────────────────────────

    public static IReadOnlyList<ItemTypeEntry> Armor { get; } =
    [
        new("Chest Armor",  0x0006D16D, "ChestArmor"),
        new("Helm",         0x0006D16E, "Helm"),
        new("Pants",        0x0006D16F, "Legs"),
        new("Boots",        0x0006D170, "Boots"),
        new("Gloves",       0x0006D171, "Gloves"),
        new("Shield",       0x0006D172, "Shield")
    ];

    // ── Accessories ──────────────────────────────────────────────────────────

    public static IReadOnlyList<ItemTypeEntry> Accessories { get; } =
    [
        new("Ring",    0x0006D174, "Ring"),
        new("Amulet",  0x0006D175, "Amulet")
    ];

    // ── Special (Charm system, Season 13+) ──────────────────────────────────

    public static IReadOnlyList<ItemTypeEntry> Special { get; } =
    [
        new("Charm",         0x0022ed05, "Charm"),
        new("Horadric Seal", 0x00237e80, "HoradricSeal")
    ];

    // ── Aggregates ───────────────────────────────────────────────────────────

    public static IReadOnlyList<ItemTypeEntry> All { get; } =
        [.. Weapons, .. Armor, .. Accessories, .. Special];

    public static IReadOnlyDictionary<uint, ItemTypeEntry> ByHash { get; } =
        All.ToDictionary(e => e.Hash);

    public static string GetDisplayName(uint hash)
        => ByHash.TryGetValue(hash, out var entry) ? entry.Name : $"Unknown item type (0x{hash:x8})";
}
