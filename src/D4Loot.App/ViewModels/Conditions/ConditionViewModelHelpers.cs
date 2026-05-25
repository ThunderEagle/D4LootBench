using D4Loot.Core.Data;
using D4Loot.Core.Models;

namespace D4Loot.App.ViewModels.Conditions;

internal static class ConditionViewModelHelpers
{
    internal static string LookupName(uint id)
    {
        if (AffixDatabase.ByHash.TryGetValue(id, out var affixEntry))
            return affixEntry.Name;
        if (SkillDatabase.ByHash.TryGetValue(id, out var skillEntry))
            return skillEntry.Name;
        if (ItemTypeDatabase.ByHash.TryGetValue(id, out var itemTypeEntry))
            return itemTypeEntry.Name;
        return $"0x{id:x8}";
    }

    internal static string LookupUniqueName(uint hash)
    {
        // In the filter wire format, unique item IDs are hash values that equal their SNO IDs
        if (UniqueItemDatabase.ByHash.TryGetValue(hash, out var entry))
            return entry.Name;
        return $"0x{hash:x8}";
    }

    internal static string FormatRarityFlags(RarityFlags flags)
    {
        if (flags == RarityFlags.All) return "All";
        var parts = new List<string>(7);
        if (flags.HasFlag(RarityFlags.Common))    parts.Add("Common");
        if (flags.HasFlag(RarityFlags.Magic))     parts.Add("Magic");
        if (flags.HasFlag(RarityFlags.Rare))      parts.Add("Rare");
        if (flags.HasFlag(RarityFlags.Legendary)) parts.Add("Legendary");
        if (flags.HasFlag(RarityFlags.Unique))    parts.Add("Unique");
        if (flags.HasFlag(RarityFlags.Mythic))    parts.Add("Mythic");
        if (flags.HasFlag(RarityFlags.Talisman))  parts.Add("Talisman");
        return parts.Count == 0 ? "None" : string.Join(", ", parts);
    }
}
