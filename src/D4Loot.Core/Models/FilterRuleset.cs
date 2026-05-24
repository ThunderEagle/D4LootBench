namespace D4Loot.Core.Models;

public sealed record FilterRuleset(
    string Name,
    IReadOnlyList<FilterRule> Rules
);
