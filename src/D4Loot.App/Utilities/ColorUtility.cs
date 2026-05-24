using System.Windows.Media;

namespace D4Loot.App.Utilities;

internal static class ColorUtility
{
    public static Color AbgrToWpf(uint abgr) => Color.FromArgb(
        a: (byte)(abgr >> 24 & 0xFF),
        r: (byte)(abgr & 0xFF),
        g: (byte)(abgr >> 8 & 0xFF),
        b: (byte)(abgr >> 16 & 0xFF));

    // HSL where H ∈ [0, 360), S/L ∈ [0, 1].
    // Channel dominance is determined on the original bytes to avoid float equality comparisons.
    public static (float H, float S, float L) AbgrToHsl(uint abgr)
    {
        var rByte = (byte)(abgr & 0xFF);
        var gByte = (byte)(abgr >> 8 & 0xFF);
        var bByte = (byte)(abgr >> 16 & 0xFF);

        var r = rByte / 255f;
        var g = gByte / 255f;
        var b = bByte / 255f;

        var maxByte = Math.Max(rByte, Math.Max(gByte, bByte));
        var minByte = Math.Min(rByte, Math.Min(gByte, bByte));
        var max     = maxByte / 255f;
        var min     = minByte / 255f;
        var delta   = max - min;
        var l       = (max + min) / 2f;
        var s       = delta == 0f ? 0f : delta / (1f - MathF.Abs(2f * l - 1f));

        var h = 0f;
        if (delta == 0f)
            return (h, s, l);

        if (maxByte == rByte)      h = 60f * ((g - b) / delta % 6f);
        else if (maxByte == gByte) h = 60f * ((b - r) / delta + 2f);
        else                       h = 60f * ((r - g) / delta + 4f);
        if (h < 0f) h += 360f;

        return (h, s, l);
    }

    public static uint HslToAbgr(float h, float s, float l)
    {
        var c = (1f - MathF.Abs(2f * l - 1f)) * s;
        var x = c * (1f - MathF.Abs(h / 60f % 2f - 1f));
        var m = l - c / 2f;

        var (r, g, b) = ((int)(h / 60f) % 6) switch
        {
            0 => (c, x, 0f),
            1 => (x, c, 0f),
            2 => (0f, c, x),
            3 => (0f, x, c),
            4 => (x, 0f, c),
            _ => (c, 0f, x)
        };

        var rb = (byte)((r + m) * 255f);
        var gb = (byte)((g + m) * 255f);
        var bb = (byte)((b + m) * 255f);

        // ABGR: A=255, B in bits 16–23, G in bits 8–15, R in bits 0–7
        return 0xFF000000u | (uint)bb << 16 | (uint)gb << 8 | rb;
    }

    /// <summary>
    /// Returns an ABGR color whose hue sits at the midpoint of the largest angular gap
    /// between <paramref name="existingColors"/> hues on the 360° wheel.
    /// Fixed S=0.85 / L=0.55 keeps colours vivid and readable as D4 filter overlays.
    /// </summary>
    public static uint GenerateDistinctColor(IEnumerable<uint> existingColors)
    {
        var hues = existingColors
            .Select(c => AbgrToHsl(c).H)
            .OrderBy(h => h)
            .ToList();

        if (hues.Count == 0)
            return HslToAbgr(200f, 0.85f, 0.55f);

        var bestMid = 0f;
        var bestGap = 0f;

        for (var i = 0; i < hues.Count; i++)
        {
            var next = i + 1 < hues.Count ? hues[i + 1] : hues[0] + 360f;
            var gap  = next - hues[i];
            if (gap <= bestGap) continue;
            bestGap = gap;
            bestMid = (hues[i] + gap / 2f) % 360f;
        }

        return HslToAbgr(bestMid, 0.85f, 0.55f);
    }
}
