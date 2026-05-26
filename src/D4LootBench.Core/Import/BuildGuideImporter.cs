namespace D4LootBench.Core.Import;

public sealed class BuildGuideImportException(string message) : Exception(message);

/// <summary>
/// Detects the format of a pasted build guide text and delegates to the appropriate parser.
/// Accepts an optional <paramref name="hint"/> to override auto-detection.
/// </summary>
public sealed class BuildGuideImporter
{
    // Known Maxroll slot keywords — used for auto-detection (first non-blank line check)
    private static readonly HashSet<string> MaxrollFirstLineKeywords = new(StringComparer.OrdinalIgnoreCase)
    {
        "Helm", "Chest Armor", "Gloves", "Pants", "Boots",
        "Amulet", "Left Ring", "Right Ring", "Mainhand", "Offhand", "Seal", "Weapon"
    };

    private readonly MobalyticsParser _mobalytics = new();
    private readonly MaxrollParser _maxroll = new();
    private readonly IcyVeinsParser _icyVeins = new();

    public ParsedBuildGuide Import(string text, BuildGuideFormat hint = BuildGuideFormat.Auto)
    {
        var format = hint == BuildGuideFormat.Auto ? Detect(text) : hint;
        return format switch
        {
            BuildGuideFormat.Mobalytics => _mobalytics.Parse(text),
            BuildGuideFormat.Maxroll    => _maxroll.Parse(text),
            BuildGuideFormat.IcyVeins  => _icyVeins.Parse(text),
            _ => throw new BuildGuideImportException("Format could not be detected. Select the format manually.")
        };
    }

    private static BuildGuideFormat Detect(string text)
    {
        if (text.Contains("toggle modifiers", StringComparison.OrdinalIgnoreCase))
            return BuildGuideFormat.Mobalytics;

        if (text.Contains("Gear Affixes\t", StringComparison.Ordinal))
            return BuildGuideFormat.IcyVeins;

        // Maxroll: first non-blank line is a known slot keyword
        foreach (var line in text.ReplaceLineEndings("\n").Split('\n'))
        {
            var trimmed = line.Trim();
            if (string.IsNullOrEmpty(trimmed)) continue;
            if (MaxrollFirstLineKeywords.Contains(trimmed))
                return BuildGuideFormat.Maxroll;
            break;
        }

        return BuildGuideFormat.Auto; // sentinel: unknown
    }
}
