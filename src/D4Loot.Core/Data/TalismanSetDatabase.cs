namespace D4Loot.Core.Data;

public sealed record TalismanSetItemEntry(string Name, uint Hash, string InternalName);

public sealed record TalismanSetInfo(string Name, uint Hash, string InternalName,
    IReadOnlyList<TalismanSetItemEntry> Items);

public static class TalismanSetDatabase
{
    public static IReadOnlyList<TalismanSetInfo> All { get; }
    public static IReadOnlyDictionary<uint, TalismanSetInfo> ByHash { get; }

    /// <summary>Flat lookup of all item hashes across every set.</summary>
    public static IReadOnlyDictionary<uint, TalismanSetItemEntry> ItemsByHash { get; }

    static TalismanSetDatabase()
    {
        var all = new List<TalismanSetInfo>();

        var arr = FilterDataStore.Root.GetProperty("talismanSets");
        foreach (var el in arr.EnumerateArray())
        {
            var name         = el.GetProperty("displayName").GetString()!;
            var internalName = el.GetProperty("internalName").GetString()!;

            // Hash field is not yet populated in d4-data.json; skip entries until data is extended
            if (!el.TryGetProperty("hash", out var hashEl))
                continue;
            var hash = Convert.ToUInt32(hashEl.GetString()![2..], 16);

            var items = new List<TalismanSetItemEntry>();
            foreach (var item in el.GetProperty("items").EnumerateArray())
            {
                var iName    = item.GetProperty("displayName").GetString()!;
                var iInternal = item.GetProperty("internalName").GetString()!;
                if (!item.TryGetProperty("hash", out var iHashEl))
                    continue;
                var iHash = Convert.ToUInt32(iHashEl.GetString()![2..], 16);
                items.Add(new TalismanSetItemEntry(iName, iHash, iInternal));
            }

            all.Add(new TalismanSetInfo(name, hash, internalName, items.AsReadOnly()));
        }

        All = all.AsReadOnly();
        ByHash = all.ToDictionary(e => e.Hash);
        ItemsByHash = all
            .SelectMany(s => s.Items)
            .ToDictionary(i => i.Hash);
    }

    public static string GetSetName(uint hash)
        => ByHash.TryGetValue(hash, out var entry) ? entry.Name : $"Unknown set (0x{hash:x8})";

    public static string GetItemName(uint hash)
        => ItemsByHash.TryGetValue(hash, out var entry) ? entry.Name : $"Unknown item (0x{hash:x8})";
}
