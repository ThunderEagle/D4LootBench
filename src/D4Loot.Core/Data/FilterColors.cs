namespace D4Loot.Core.Data;

/// <summary>Standard D4 filter highlight colors in ABGR packed uint32 format.</summary>
public static class FilterColors
{
    // Packed as: A=bits 24-31, B=bits 16-23, G=bits 8-15, R=bits 0-7
    public const uint Default = 0xFFFF0000; // R=0,   G=0,   B=255  (blue)
    public const uint Cyan    = 0xFFFFFF00; // R=0,   G=255, B=255
    public const uint Green   = 0xFF00C800; // R=0,   G=200, B=0
    public const uint Orange  = 0xFF008CFF; // R=255, G=140, B=0
    public const uint Gold    = 0xFF00D7FF; // R=255, G=215, B=0
}
