using System.Text.Json.Serialization;

namespace D4Loot.Core.Models;

public sealed record FilterRule(
    string Name,
    Visibility Visibility,
    uint Color,
    IReadOnlyList<Condition> Conditions,
    bool IsEnabled = true
)
{
    /// <summary>Deconstructs the packed ABGR uint into individual channels.</summary>
    [JsonIgnore]
    public (byte R, byte G, byte B, byte A) ColorChannels => (
        R: (byte)(Color & 0xFF),
        G: (byte)((Color >> 8) & 0xFF),
        B: (byte)((Color >> 16) & 0xFF),
        A: (byte)((Color >> 24) & 0xFF)
    );

    public static uint PackColor(byte r, byte g, byte b, byte a = 255)
        => ((uint)a << 24) | ((uint)b << 16) | ((uint)g << 8) | r;
}
