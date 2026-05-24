# D4Loot — Project Context

## What This Is
A standalone WPF desktop application for editing Diablo IV loot filter share codes. D4's in-game filter UI is clunky; this app lets players import a filter code, visually edit all its rules, then re-export the code to paste back into the game. Distribution via GitHub Releases as a self-contained single-file `.exe` — no installer, no hosting required.

## Technology Stack
- **.NET 10 / WPF** (`net10.0-windows`) — Windows-only desktop app, user's wheelhouse
- **CommunityToolkit.Mvvm 8.4.2** — MVVM source generators (D4Loot.App)
- **Shouldly 4.3.0** — test assertions (MIT license)
- **xUnit** — test runner

## Solution Layout
```
D4Loot.sln
├── src/D4Loot.Core/          # Pure .NET 10 class library — zero WPF dependency
│   ├── Models/               # FilterRuleset, FilterRule, Condition subtypes, Enums
│   ├── Codec/                # FilterCodec (encode/decode), ProtoWriter, ProtoReader
│   └── Data/                 # AffixDatabase, SkillDatabase, FilterColors
├── src/D4Loot.Ai/            # (Phase 4) LLM provider abstraction — no WPF dependency
│   ├── ILlmProvider.cs
│   ├── LlmSettings.cs
│   ├── RuleAssistant.cs
│   └── Providers/            # OllamaProvider, AnthropicProvider, OpenAiProvider
├── src/D4Loot.App/           # WPF app
│   ├── ViewModels/           # (Phase 2 — not yet built)
│   └── Views/                # (Phase 2 — not yet built)
├── tests/D4Loot.Core.Tests/
│   └── Codec/FilterCodecTests.cs   # 12 passing tests
└── docs/
    ├── filter-format.md      # Full protobuf format spec with field tables and hash IDs
    └── ai-assistant.md       # AI rule assistant architecture and design decisions
```

## Filter Code Format (Critical Background)
D4 share codes are **Base64-encoded hand-rolled Protocol Buffers binary** — no compression.
Full spec is in `docs/filter-format.md`. Key points:
- **Filter** → repeated Rule messages (field 1) + name (field 2) + count (field 3) + version=1 (field 4)
- **Rule** → name (1), visibility/enum (2), color/ABGR-uint32 (3), repeated Condition (4), enabled (5)
- **Condition** types (all 10 known): Item Power (0), Rarity (1), Item Properties (2), Greater Affix (3), Codex (4), Item Type (5), Required Affixes (6), Optional Affixes (7), Specific Unique (8), Talisman Set (9)
- Types 0–7 are fully modelled; types 8–9 round-trip as `UnknownCondition` (IDs not yet catalogued)
- Color format: packed ABGR `uint32` little-endian — `makeColor(r,g,b)` = `(a<<24)|(b<<16)|(g<<8)|r`
- Rules are written in **reverse display order** (lowest-priority rule first in binary)
- **Maximum 25 rules per filter** — game-enforced limit; editor must validate on export
- 63 confirmed affix hash IDs in `AffixDatabase`; full skill IDs for all 9 classes in `SkillDatabase` (4 Sorcerer basic skills pending in-game name verification)
- Item type IDs fully catalogued (25 types): Charm = `0x0022ed05`, Seal = `0x00237e80`

Sources: Upsilon72/d4-filter-generator (Season 13), fnuecke/diablo4-loot-filter-viewer (.proto), DiabloTools/d4data (CoreTOC)

## Attribution Required (Before Public Release)
- **Upsilon72/d4-filter-generator** (MIT) — original protobuf wire format reverse engineering, condition type encoding, affix hash IDs
- **fnuecke/diablo4-loot-filter-viewer** (Unlicense/public domain) — complete `.proto` field layout, all 10 condition type semantics, `names.json` ID lookup
- **DiabloTools/d4data** (MIT) — `CoreTOC_flat.json`, authoritative datamined ID tables for all skills, item types, and affixes
- **d4lfteam/d4lf** (MIT) — affix name reference database
- **Raxx** (filter author) — real-world filter export used to validate and extend the spec
- Must appear in app About dialog and README. See `docs/filter-format.md` for full wording and license status.

## What's Done
- **Phase 0** ✅ — Format fully reverse-engineered; `docs/filter-format.md` written; all 10 condition types documented
- **Phase 1** ✅ — Core library complete:
  - Domain models (`FilterRuleset`, `FilterRule`, full `Condition` hierarchy — 9 types)
  - `FilterCodec.Encode()` / `FilterCodec.Decode()` — bidirectional, lossless round-trip for all condition types
  - `AffixDatabase` (63 entries), `SkillDatabase` (all 9 classes, ~200 entries), `ItemTypeDatabase` (25 types), `FilterColors`
  - 15 unit tests passing, 0 warnings
  - Attribution sources confirmed; all licenses verified

## What's Next
- **Phase 2** — WPF shell: main window + JSON editor tab (AvalonEdit, round-trip import/export), then rule list and rule editor panel
- **Phase 3** — Item/affix data integration: condition value pickers bind to AffixDatabase/SkillDatabase; resolve 4 Sorcerer basic skill display names
- **Phase 4** — AI rule assistant: `D4Loot.Ai` project, Ollama-first, optional cloud providers (see `docs/ai-assistant.md`)

## Key Decisions Made
- **WPF over MAUI** — audience is 100% Windows, user's comfort zone, simpler deployment
- **Custom protobuf codec** over Google.Protobuf — format uses only 3 wire types, ~80 lines, handles unknown fields gracefully for patch resilience
- **Shouldly** over FluentAssertions — FA v8 went commercial; Shouldly stays MIT
- **No Priority field on FilterRule** — priority is implicit from list index; redundant field would create inconsistency
- **UnknownCondition type** preserves raw bytes for condition types not yet mapped, ensuring lossless round-trips on future game patches
- **JSON editor before visual editor** — AvalonEdit tab gives immediate insight into filter structure; doubles as a power-user/debug feature in the final app
- **AI assistant is opt-in and user-configured** — not bundled with a hardcoded API key; users choose their provider (Ollama free/local, or cloud with own key); see `docs/ai-assistant.md`
- **Ollama-first for AI** — local/free, zero key management, validates the full assistant UX loop before adding cloud provider complexity

## Running / Testing
```powershell
dotnet build          # full solution
dotnet test           # 12 tests in D4Loot.Core.Tests
```

## Publish (Phase 4)
```powershell
dotnet publish src/D4Loot.App -r win-x64 -p:PublishSingleFile=true --self-contained true
```

## README (not yet created) — Doc reminders for initial write
When creating the repo's README.md, include a troubleshooting section noting that if a share code imported from an external source fails to decode or produces unexpected results, the user should re-export the filter from the in-game UI and use that fresh code instead. Older tool exports or manually-shared codes may have subtle encoding differences (e.g. the GitHub copy of Raxx's filter has 13 greater entries per condition instead of the game's 14).
