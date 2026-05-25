# D4Loot Data Gaps

## Overview

This document tracks known gaps in the d4-data.json database and provides guidance on prioritization and resolution.

**Current Status:** Core filter functionality is complete and tested. Gaps are secondary and can be resolved reactively.

---

## Gap 1: Affixes (RESOLVED ✅)

### Status
- **Coverage:** 251 documented affixes (was 63)
- **Estimated Total:** 236 S04_ affixes + 15 X2/S11/S12 affixes = fully covered for standard item affixes

### What We Have
Complete S04_ affix set (all 236 type-104 S04_ entries from CoreTOC) including:
- All core stat and combat affixes (Armor, AttackSpeed, CritChance, etc.)
- Stat variants: ArmorPercent, AllStats, % Dexterity/Intelligence/Strength/Willpower
- All 102 named SkillRankBonus affixes (+Whirlwind, +Fireball, etc.) — resolved via d4data Power STL files
- 15 SkillRankBonus category affixes (Brawling Skills (Barbarian), etc.)
- All 52 PassiveRankBonus affixes — resolved via Power Talent STL files
- Resource affixes (On Kill, Per Second) for all 9 classes
- 15 non-S04 affixes: X2 Talisman-era damage multipliers, S11 stats, Evade mechanics, Weapon Damage

### Display Name Resolution Method
1. **SkillRankBonus (named):** `.aff.json` → `nParam` → CoreTOC SNO ID → `Power_{Class}_{Skill}.stl.json` → `[Name]`
2. **SkillRankBonus (category):** Derived from internal name pattern
3. **PassiveRankBonus:** Internal name → `Power_{Class}_Talent_{path}.stl.json` → `[Name]`
4. **Stat affixes:** Manually curated display names

### What's Still Missing
- X2/S11/S12/S13 tier affixes (250 entries): primarily Transfiguration affixes, Kill Streak mechanics, Greater stat variants, Warlock/Paladin/Spiritborn skill ranks
- These are low priority — mostly Horadric Cube Transfiguration and seasonal mechanics players rarely filter on

### Sorcerer Basics Confirmed
- `S04_SkillRankBonus_Sorc_Basic_1` = Spark
- `S04_SkillRankBonus_Sorc_Basic_2` = Fire Bolt
- `S04_SkillRankBonus_Sorc_Basic_3` = Frost Bolt
- `S04_SkillRankBonus_Sorc_Basic_4` = Arc Lash

### Priority
**RESOLVED** — Standard item affixes fully covered. Remaining gaps are Transfiguration and seasonal mechanics.

---

## Gap 2: Skills (Low-Medium Priority)

### Status
- **Coverage:** 223 skills across 9 classes
- **Estimated Total:** 250+ skills (including variants)

### What We Have
- All 9 classes documented (Barbarian, Sorcerer, Necromancer, Druid, Rogue, Paladin, Spiritborn, Warlock)
- ~200 core verified and datamined skills per AGENTS.md
- Coverage includes new Warlock class (Season 13)

### What's Missing
- Season 13 additions (Warlock skills are included but may be incomplete)
- Skill variants and specializations
- Passive skills (if used in filter conditions)
- Unverified/datamined skills that haven't been confirmed in-game

### Impact on Filters
**Low** — Most player filters focus on affixes and item properties. Skill-based conditions are less common.
Unknown skills display as `Unknown (0x...)` but filters decode fully.

### How to Resolve
1. **Verify Coverage:** Decode filters from Season 13 players; check for unknown skills
2. **Extract All:** Use d4data to populate complete skill database with all variants
3. **Verify Status:** Mark each skill as "verified" or "datamined" for transparency

### Priority
**MEDIUM** — Verify Warlock completeness before public release. Full coverage can be added post-launch.

---

## Gap 3: Affixes for Unique/Talisman Items (Medium Priority)

### Status
- **Coverage:** Basic affix database (63 entries)
- **Issue:** Affixes on specific unique items or talismans not separately tracked

### What We Have
- Unique items fully resolved (901 entries, 848 with display names)
- Talisman sets fully resolved (50 sets)
- Generic affix database

### What's Missing
- **Not a data gap, but a modeling question:** Should we track which affixes appear on specific uniques/talismans?
  - Currently: No. The filter format just references the item by ID.
  - Could add later: Item → {possible affixes} mapping for UI tooltips/suggestions

### Impact on Filters
**None** — Filters don't care what affixes *could* appear on an item; they just match the item type.

### Priority
**LOW** — Nice-to-have for UI suggestions, not required for core functionality.

---

## Gap 4: Item Type Affixes (Very Low Priority)

### Status
- **Coverage:** 27 item types (complete)
- **Issue:** Which affixes can roll on which item types?

### What We Have
- All 27 item types (Axe, Bow, Chest Armor, Ring, etc.) with hashes and names
- Full codec support

### What's Missing
- Item type → {allowed affixes} mapping
- Affix → {allowed item types} reverse mapping

### Impact on Filters
**None** — Filters don't validate affix-item-type combinations; they just encode/decode.

### Priority
**VERY LOW** — Could be a nice UI feature (warn users of impossible affix-item combos) but not critical.

---

## Gap 5: Affixes with Minimum/Maximum Values (Medium Priority)

### Status
- **Coverage:** None documented in d4-data.json

### What We Have
- Affix hash IDs and display names

### What's Missing
- Min/max roll values for each affix (e.g., "Armor rolls 0–500")
- Seasonal stat caps
- Class-specific affix ranges

### Impact on Filters
**Low** — Filters don't encode specific affix values in the current condition format.
If value-based affix filtering is added in the future, this becomes critical.

### Priority
**MEDIUM** — Document for completeness; implement if/when value-based conditions are added to the game.

---

## Gap 6: Undocumented Items (Very Low Priority)

### Status
- **Coverage:** 901 unique items (85.4% released, rest are dev/unreleased)
- **Issue:** 53 items have unresolved or placeholder display names

### What We Have
- 848 unique items with full player-friendly display names
- 46 placeholder `[PH]` items (clearly unreleased)
- 7 unresolved items (mostly internal/non-player-facing)

### What's Missing
- **Placeholder items:** These are intentionally cut content; no action needed
- **Unresolved items:** 4 Necromancer items with no StringList, 2 deleted assets, 1 internal (Horadric Cube ingredient)

### Impact on Filters
**None** — These items rarely/never appear in real filters. If they do, they display as `Unknown unique (0x...)`.

### Priority
**VERY LOW** — Complete only if players report missing uniques in their filters.

---

## Gap 7: Talisman Item Mappings (Low Priority)

### Status
- **Coverage:** 50 talisman sets with display names
- **Issue:** Which items belong to each talisman set?

### What We Have
```json
{
  "displayName": "Berserker's Crucible",
  "internalName": "Talisman_Barb_02.stl",
  "items": []  // ← Empty!
}
```

### What's Missing
- Item → set membership mapping
- For each talisman set, list the specific unique items (charms/talisman conversions) that are part of it

### Impact on Filters
**None** — The filter codec already decoded Berserker's Crucible correctly (field 3 contains item mappings).
This is just for display/UI enhancement (showing "which items are in this set").

### How to Resolve
1. Extract from `Item_Talisman_Charm_Set_*.stl.json` files (already found in d4data)
2. Parse to find talisman charm definitions
3. Map back to set names

### Priority
**LOW** — UI feature, not core functionality. Can be added post-launch.

---

## Summary Table

| Gap | Category | Items | Coverage | Impact | Priority |
|-----|----------|-------|----------|--------|----------|
| 1 | Affixes (S04 + X2/S11) | 251 / 501 | 50% | Low | ✅ RESOLVED (standard affixes complete) |
| 2 | Skills | 223 / 250+ | 89% | Low | MEDIUM |
| 3 | Unique Affix Maps | 0 / TBD | 0% | None | LOW |
| 4 | Item Type Affix Maps | 0 / TBD | 0% | None | VERY LOW |
| 5 | Affix Value Ranges | 0 / TBD | 0% | Low | MEDIUM |
| 6 | Undocumented Items | 7 / 901 | 99.2% | None | VERY LOW |
| 7 | Talisman Item Maps | 0 / 50 | 0% | None | LOW |

---

## Recommended Action Plan

### Before Public Release (MVP)
✅ **COMPLETE** — All critical gaps closed:
- Unique items: 901 (99.2% resolved)
- Talisman sets: 50 (100% resolved)
- Item types: 27 (100% resolved)
- Skills: 223 (89% coverage)
- Affixes: 251 (all standard S04 item affixes covered, including 102 named skill ranks + 52 passives)

### Post-Launch (Phase 2)
1. **Gap 2:** Verify Warlock skill completeness; collect unknown skills from player filters
2. **Gap 1:** Expand affix database reactively as players encounter unknowns
3. **Gap 5:** If value-based affix filtering is added, document affix ranges
4. **Gap 7:** Extract talisman item mappings for set tooltips/UI enhancement

### Never (Low ROI)
- Gaps 3, 4, 6 — Minimal impact, high effort

---

## Testing Coverage

All gaps have been validated against:
- ✅ 23 unit tests (100% passing)
- ✅ Real filter sample (Berserker's Crucible talisman set decoding)
- ✅ All 10 condition types
- ✅ Lossless encode/decode round-trips

Unknown items/affixes/skills in filters:
- Display as `Unknown (0x{hash:x8})`
- Do not break filter decoding
- Do not affect round-trip integrity
- Can be logged for gap-filling analysis

---

## How to Report / Extend

### Reporting a Gap
If a player filter contains an unknown item/affix/skill:
1. Decode the filter (app logs unknown items)
2. Extract the hash ID from the log
3. File an issue with:
   - Filter code
   - Hash ID
   - Item/affix/skill name (if known from in-game)
   - How to reproduce

### Extending the Database
To add new items/affixes/skills:
1. Extract from d4data StringList files
2. Lookup hash ID from fnuecke's `names.json` or internal name
3. Verify against real game data or player filters
4. Add to `src/D4Loot.Core/Data/d4-data.json`
5. Run tests to ensure no regressions
6. Commit with clear attribution

---

## References

- **D4Data:** https://github.com/DiabloTools/d4data
  - StringList files: `json/enUS_Text/meta/StringList/*.stl.json`
  - Authority for player-facing names
- **Fnuecke's Loot Filter Viewer:** https://github.com/fnuecke/diablo4-loot-filter-viewer
  - `names.json` — SNO ID mappings
  - Reference `.proto` file
- **D4LF:** https://github.com/d4lfteam/d4lf
  - Real-world filter usage patterns
- **Project AGENTS.md:** Lists known data coverage by category

---

## Last Updated
May 24, 2026 — Affix database expanded to 251 entries (S04 complete: all named skill ranks + passives resolved from d4data Power STL files)
