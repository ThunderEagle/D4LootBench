# FilterForge — Project Context

## What This Is
A standalone WPF desktop application for editing Diablo IV loot filter share codes. D4's in-game filter UI is clunky; this app lets players import a filter code, visually edit all its rules, then re-export the code to paste back into the game. Distribution via GitHub Releases as a self-contained single-file `.exe` — no installer, no hosting required.

## Technology Stack
- **.NET 10 / WPF** (`net10.0-windows`) — Windows-only desktop app
- **CommunityToolkit.Mvvm 8.4.2** — MVVM source generators (FilterForge.App)
- **Microsoft.Extensions.DependencyInjection 10.0.0** — DI container for the App
- **AvalonEdit 6.3.0** — JSON editor with syntax highlighting, folding, search
- **Shouldly 4.3.0** — test assertions
- **xUnit** — test runner

## Solution Layout
```
FilterForge.slnx
├── src/FilterForge.Core/          # Pure .NET 10 class library — zero WPF dependency
│   ├── Models/               # FilterRuleset, FilterRule, 10 Condition subtypes + UnknownCondition
│   ├── Codec/                # FilterCodec (encode/decode), ProtoWriter, ProtoReader
│   ├── Data/                 # IFilterDataService + per-category catalogs, *Database statics, d4-data.json
│   ├── Validation/           # IFilterValidator, FilterValidator, ValidationResult
│   └── Serialization/        # FilterJsonOptions, HexUInt32Converter, annotated {id,name} converters, FilterDataContext
├── src/FilterForge.Ai/            # Pure .NET 10 class library — no WPF dependency
│   ├── ILlmProvider.cs       # Core abstraction (GetCompletionAsync)
│   ├── LlmSettings.cs        # Provider enum + config model (BaseUrl, ModelName, ApiKey)
│   ├── LlmCompletion.cs      # Result wrapper (Content, Error, IsSuccess)
│   ├── RuleGenerationResult.cs # Success/failure + Rule + Suggestions + Warnings
│   ├── RuleAssistant.cs      # Orchestrates prompt → provider → parse → resolve → validate
│   ├── SystemPromptBuilder.cs # Builds/caches system prompt from live catalogs
│   ├── NameResolver.cs       # Name → hash ID resolution with fuzzy fallback
│   └── Providers/
│       ├── OllamaProvider.cs # HTTP to localhost OpenAI-compat endpoint
│       └── MockLlmProvider.cs # Hardcoded response for UI dev / test mode
├── src/FilterForge.App/           # WPF app
│   ├── ViewModels/           # MainWindowVM, VisualEditorVM, FilterRuleVM, RawEditorVM, ColorPickerVM, AiAssistantVM, Conditions/*
│   ├── Views/                # VisualEditorView, RawEditorWindow, ColorPickerDialog, IssuesPanel, AiAssistantView
│   ├── Behaviors/            # ScrollNewItemsIntoView attached behavior
│   ├── Converters/           # BoolToBrushConverter, ValidationSeverityConverter
│   ├── Services/             # ServiceConfiguration, LlmSettingsService, LlmProviderFactory, SettingsAwareLlmProvider
│   └── Utilities/            # ColorUtility (HSV/ABGR conversion, contrast helper)
├── tests/FilterForge.Core.Tests/
│   ├── Codec/                # FilterCodecTests — round-trip, real Raxx filter, idempotency
│   ├── Validation/           # FilterValidatorTests — 19 tests for limits, boundaries, indices
│   ├── SerializationTests/   # AnnotatedJsonTests — id-wins, name-only, legacy form, unknown hash
│   └── TestSetup.cs          # ModuleInitializer that wires FilterDataContext for tests
├── docs/
│   ├── filter-format.md      # Full protobuf spec with field tables and hash IDs
│   ├── d4-data-format.md     # d4-data.json schema reference (for community edits)
│   ├── reference-codes/      # Raw Base64 share codes (Raxx, wudijo, crit-filter, GameRant)
│   └── design/               # Archived design docs and phase history
│       ├── phase-history.md  # Per-phase build narrative (Phases 0–4A)
│       ├── visual-editor.md  # Phase 2 design decisions
│       ├── ai-assistant.md   # Phase 4 design decisions
│       └── data-gaps.md      # Data gap analysis and resolution notes
└── json-filters/             # Reference fixtures (Raxx filter, All Conditions Test)
```

## Current State
All phases complete (0–4A). **58 tests**, 0 warnings. See `docs/design/phase-history.md` for the full build narrative.

## Filter Code Format (Critical Background)
D4 share codes are **Base64-encoded hand-rolled Protocol Buffers binary**. Full spec in `docs/filter-format.md`. Key points:
- **Filter** → repeated Rule messages (field 1) + name (field 2) + count (field 3) + version=1 (field 4)
- **Rule** → name (1), visibility/enum (2), color/ABGR-uint32 (3), repeated Condition (4), enabled (5)
- **Condition** types (all 10 known + `UnknownCondition` defensive fallback): Item Power (0), Rarity (1), Item Properties (2), Greater Affix (3), Codex (4), Item Type (5), Required Affixes (6), Optional Affixes (7), Specific Unique (8), Talisman Set (9)
- All 10 condition types are fully modelled with codec support and per-type editor ViewModels
- Color format: packed ABGR `uint32` little-endian
- Rules are written in **reverse display order** (lowest-priority rule first in binary)
- **Maximum 25 rules per filter** — game-enforced limit; pre-emptive validation disables Copy Code when violated
- 251 affix hash IDs, ~200 skills (9 classes), 27 item types, ~900 unique items (~848 display names resolved)

Sources: Upsilon72/d4-filter-generator, fnuecke/diablo4-loot-filter-viewer, DiabloTools/d4data, d4lfteam/d4lf

## JSON Output Format (Annotated)
Filter JSON emits hash IDs as `{ "id": "0x…", "name": "…" }` objects across affixes, item types, uniques, talisman sets, plus name siblings on `GreaterAffixEntry` and `TalismanSetEntry`. Hash IDs remain authoritative — names are informational. On read: `id` wins when present; `id` missing falls back to name lookup; mismatched `id`+`name` prefers `id` (validator surfaces a warning). Legacy string-hash form (`"AffixIds": ["0x…"]`) still deserializes. Converters resolve names through `FilterDataContext.Current`, set once at app startup.

## Key Decisions
- **WPF over MAUI** — audience is 100% Windows, simpler deployment
- **Custom protobuf codec** over Google.Protobuf — 3 wire types, ~80 lines, handles unknown fields for patch resilience
- **Shouldly** over FluentAssertions — FA v8 went commercial; Shouldly stays MIT
- **UnknownCondition** — preserves raw bytes for future/prototype condition types, ensuring lossless round-trips
- **Per-type ViewModels** — each condition type gets its own editor ViewModel + DataTemplate
- **DI via Microsoft.Extensions.DependencyInjection** — standard; `SettingsAwareLlmProvider`, `SystemPromptBuilder`, `NameResolver`, `RuleAssistant` registered as singletons
- **`SettingsAwareLlmProvider` singleton** — lets `RuleAssistant` be a singleton while provider selection changes at runtime; reads `LlmSettingsService.Current` on each call
- **DPAPI for API key storage** — `ProtectedData.Protect/Unprotect` with `DataProtectionScope.CurrentUser`; in-box on `net10.0-windows`, no extra NuGet package needed
- **No hardcoded API key** — users bring their own Ollama instance; Ollama is the recommended free path
- **Annotated JSON over wire form** — `{id, name}` makes the format human-editable AND lets an LLM reason about content; old string-hash form still reads for backward compat
- **Static `FilterDataContext` for JSON converters** — STJ reflectively constructs converters with no ctor args, so the data service is reached via a narrow set-once static
- **Phantom `% X` primary stats removed from `d4-data.json`** — hashes `0x001d5ded..0x001d5df5` exist in CoreTOC but D4's filter editor doesn't expose them. See `docs/filter-format.md`.
- **Panel collapse via row heights** — `Visibility=Collapsed` on fixed-height Grid rows doesn't reclaim space; code-behind sets row heights to 0 instead. Last user-dragged height preserved.

## Running / Testing
```powershell
dotnet build          # full solution (0 warnings)
dotnet test           # 58 tests in FilterForge.Core.Tests
dotnet publish src/FilterForge.App -r win-x64 -p:PublishSingleFile=true --self-contained true
```

## Ad-Hoc Verification
Use `dotnet run verify.cs` (no extra install — built into .NET 10) for one-off C# scripts that reference the solution. Write a top-level statement file, add project references inline, and run. Do NOT use `dotnet script` — that requires installing a separate global tool. Prefer a temporary xunit test or `dotnet run verify.cs` over standalone scripts.

## Attribution Required (Before Public Release)
See `docs/filter-format.md` for full wording. Sources: Upsilon72/d4-filter-generator (MIT), fnuecke/diablo4-loot-filter-viewer (Unlicense), DiabloTools/d4data (MIT), d4lfteam/d4lf (MIT), Raxx (real-world filter).

## README Troubleshooting Note
When writing README.md: include a troubleshooting note that if a share code imported from an external source fails to decode or produces unexpected results, the user should re-export the filter from the in-game UI and use that fresh code. Older tool exports or manually-shared codes may have subtle encoding differences (e.g. the GitHub copy of Raxx's filter has 13 greater entries per condition instead of the game's 14).
