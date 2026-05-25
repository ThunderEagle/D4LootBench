# D4Loot — Project Context

## What This Is
A standalone WPF desktop application for editing Diablo IV loot filter share codes. D4's in-game filter UI is clunky; this app lets players import a filter code, visually edit all its rules, then re-export the code to paste back into the game. Distribution via GitHub Releases as a self-contained single-file `.exe` — no installer, no hosting required.

## Technology Stack
- **.NET 10 / WPF** (`net10.0-windows`) — Windows-only desktop app
- **CommunityToolkit.Mvvm 8.4.2** — MVVM source generators (D4Loot.App)
- **AvalonEdit 6.3.0** — JSON editor with syntax highlighting, folding, search
- **Shouldly 4.3.0** — test assertions
- **xUnit** — test runner

## Solution Layout
```
D4Loot.slnx
├── src/D4Loot.Core/          # Pure .NET 10 class library — zero WPF dependency
│   ├── Models/               # FilterRuleset, FilterRule, 10 Condition subtypes + UnknownCondition
│   ├── Codec/                # FilterCodec (encode/decode), ProtoWriter, ProtoReader
│   ├── Data/                 # AffixDatabase, SkillDatabase, ItemTypeDatabase, UniqueItemDatabase, FilterColors, FilterDataStore, d4-data.json
│   └── Serialization/        # FilterJsonOptions, HexUInt32Converter
├── src/D4Loot.App/           # WPF app (CommunityToolkit.Mvvm)
│   ├── ViewModels/           # MainWindowVM, VisualEditorVM, FilterRuleVM, ConditionVM, RawEditorVM, ColorPickerVM
│   ├── Views/                # VisualEditorView, RawEditorWindow, ColorPickerDialog
│   ├── Converters/           # BoolToBrushConverter
│   └── Utilities/            # ColorUtility (HSV/ABGR conversion, contrast helper)
├── tests/D4Loot.Core.Tests/
│   └── Codec/FilterCodecTests.cs   # 33 tests (round-trip, real Raxx filter, idempotency, hash ID test)
├── docs/
│   ├── filter-format.md      # Full protobuf spec with field tables and hash IDs
│   ├── visual-editor.md      # Visual editor UI architecture plan
│   ├── ai-assistant.md       # AI rule assistant architecture (deferred)
│   ├── share-codes.md        # Share code format overview
│   └── data-gaps.md          # Data gaps analysis and mitigation plan
├── json-filters/
│   ├── Raxx's Torment 6+ Filter.json
│   └── All Conditions Test.json
└── docs/reference-codes/
    └── raxx-torment-6-plus.txt
```

## Filter Code Format (Critical Background)
D4 share codes are **Base64-encoded hand-rolled Protocol Buffers binary**. Full spec in `docs/filter-format.md`. Key points:
- **Filter** → repeated Rule messages (field 1) + name (field 2) + count (field 3) + version=1 (field 4)
- **Rule** → name (1), visibility/enum (2), color/ABGR-uint32 (3), repeated Condition (4), enabled (5)
- **Condition** types (all 10 known + `UnknownCondition` defensive fallback): Item Power (0), Rarity (1), Item Properties (2), Greater Affix (3), Codex (4), Item Type (5), Required Affixes (6), Optional Affixes (7), Specific Unique (8), Talisman Set (9)
- All 10 condition types are fully modelled with codec support and per-type editor ViewModels
- Color format: packed ABGR `uint32` little-endian
- Rules are written in **reverse display order** (lowest-priority rule first in binary)
- **Maximum 25 rules per filter** — game-enforced limit; editor validates on export
- 251 affix hash IDs, ~200 skills (9 classes), 27 item types, ~900 unique items (~848 display names resolved)

Sources: Upsilon72/d4-filter-generator, fnuecke/diablo4-loot-filter-viewer, DiabloTools/d4data, d4lfteam/d4lf

## Phase Status
- **Phase 0** ✅ — Format reverse-engineered; `docs/filter-format.md` written; all 10 condition types documented
- **Phase 1** ✅ — Core library: domain models, codec, databases (251 affixes, ~200 skills, 27 item types, ~900 uniques), 33 tests passing, 0 warnings
- **Phase 2** ✅ — WPF shell: main window with tabs, visual editor (rule list + editor panel + color picker), JSON editor (AvalonEdit), import/export/copy/save
- **Phase 3** ✅ — Item/affix data integration: per-type condition editing ViewModels via DataTemplate dispatch, value pickers bound to databases, class filtering, selection limits/validation, unique display name resolution (848/901), greater affix picker, talisman set editing
- **Phase 4** ❌ — AI rule assistant: not started (scaffolding was removed; design doc exists at `docs/ai-assistant.md`)

## Key Decisions
- **WPF over MAUI** — audience is 100% Windows, simpler deployment
- **Custom protobuf codec** over Google.Protobuf — 3 wire types, ~80 lines, handles unknown fields for patch resilience
- **Shouldly** over FluentAssertions — FA v8 went commercial; Shouldly stays MIT
- **UnknownCondition** — preserves raw bytes for future/prototype condition types, ensuring lossless round-trips
- **Per-type ViewModels** — each condition type gets its own editor ViewModel + DataTemplate, avoids monolithic switch
- **JSON editor before visual editor** — AvalonEdit tab gives immediate insight; doubles as power-user/debug feature

## Running / Testing
```powershell
dotnet build          # full solution (0 warnings)
dotnet test           # 33 tests in D4Loot.Core.Tests
dotnet publish src/D4Loot.App -r win-x64 -p:PublishSingleFile=true --self-contained true
```

## Attribution Required (Before Public Release)
See `docs/filter-format.md` for full wording. Sources: Upsilon72/d4-filter-generator (MIT), fnuecke/diablo4-loot-filter-viewer (Unlicense), DiabloTools/d4data (MIT), d4lfteam/d4lf (MIT), Raxx (real-world filter).
