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
            var name        = el.GetProperty("displayName").GetString()!;
            var hashHex     = el.GetProperty("hash").GetString()!;
            var hash        = Convert.ToUInt32(hashHex[2..], 16);
            var internalName = el.GetProperty("internalName").GetString()!;

            var items = new List<TalismanSetItemEntry>();
            foreach (var item in el.GetProperty("items").EnumerateArray())
            {
                var iName    = item.GetProperty("displayName").GetString()!;
                var iHashHex = item.GetProperty("hash").GetString()!;
                var iHash    = Convert.ToUInt32(iHashHex[2..], 16);
                var iInternal = item.GetProperty("internalName").GetString()!;
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
