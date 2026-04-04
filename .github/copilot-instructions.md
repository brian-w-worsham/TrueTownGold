# TrueTownGold — Copilot Instructions

## Project Overview

Bannerlord mod that raises the minimum trade gold available in towns based on prosperity. The implementation is campaign-behavior driven: `TrueTownGoldBehavior` listens to `DailyTickTownEvent` and tops towns up to a prosperity-based floor using `Town.ChangeGold(...)`. No game files are modified, and there is currently no settings UI or external config file.

## Tech Stack

- **Language:** C# 9.0 targeting .NET Framework 4.7.2
- **Game SDK:** TaleWorlds Mount & Blade II: Bannerlord (`TaleWorlds.Core`, `TaleWorlds.CampaignSystem`, `TaleWorlds.Library`, `TaleWorlds.MountAndBlade`, `TaleWorlds.Localization`, `TaleWorlds.ObjectSystem`)
- **Patching:** Harmony 2.2.2 is initialized in `SubModule`, though current gameplay behavior is implemented through a `CampaignBehaviorBase`
- **Testing:** xUnit 2.6.6
- **Nullable:** Disabled project-wide

## Build, Test & Deploy Commands

```powershell
# Build
dotnet build src\TrueTownGold\TrueTownGold.csproj -c Release

# Run tests
dotnet test tests\TrueTownGold.Tests\TrueTownGold.Tests.csproj

# Deploy to game
.\deploy.ps1
```

Both the main project and test project depend on the `GameFolder` MSBuild property pointing at a valid local Bannerlord installation. If the default path is wrong, update it in both `.csproj` files or pass it explicitly during build and test.

## Architecture

| File | Role |
|------|------|
| `SubModule.cs` | Mod entry point — initializes Harmony on load, registers `TrueTownGoldBehavior` for campaign games, unpatches on unload |
| `TownGoldCalculator.cs` | Pure calculation logic for target town gold and required daily top-up |
| `TrueTownGoldBehavior.cs` | `CampaignBehaviorBase` — listens to `DailyTickTownEvent`, guards non-town cases, and applies gold increases |

### Key Design Decisions

- **CampaignBehaviorBase pattern:** The feature is implemented with `CampaignGameStarter.AddBehavior()` in `OnGameStart`, not through Harmony patches. Preserve this unless Bannerlord API limitations force a different approach.
- **Daily town tick only:** Gold updates happen through `CampaignEvents.DailyTickTownEvent`, which keeps the mod predictable and avoids per-frame or per-visit work.
- **Guard chain in `EnsureTownHasTradeGold`:** Return early if `town` is null, `Settlement` is missing, or `Settlement.IsTown` is false. Castles and invalid state should be ignored.
- **Top-up, not overwrite:** The mod calculates a target floor and only adds the missing difference. If a town already has more gold than the target, leave it unchanged.
- **Pure calculator logic:** Keep the prosperity formula and clamping behavior in `TownGoldCalculator` so it remains easy to test without live Bannerlord state.
- **No save data:** `SyncData` is intentionally a no-op. Do not introduce persisted state unless the feature genuinely needs it.
- **No player settings today:** There is no MCM integration, no config file, and no console command surface. If settings are introduced later, update both the README and this instruction file.
- **Single Harmony ID:** Use `"com.truetowngold.bannerlord"` consistently if Harmony patches are added in the future.

### Tunable Constants

| Constant | Default | Description |
|---|---|---|
| `GoldPerProsperity` | `10f` | Prosperity multiplier used to derive the target gold floor |
| `MinimumTownGold` | `15000` | Hard lower bound for town gold |
| `MaximumTownGold` | `150000` | Hard upper bound for town gold |

## Code Conventions

- **Namespace:** `TrueTownGold` at the root
- **XML documentation:** Keep `<summary>` comments on public and internal types and methods, matching the current project style
- **Game-facing guard logic:** Check for null Bannerlord objects before dereferencing them; assume campaign objects may be absent in tests or version-shifted environments
- **Calculator isolation:** Put math and clamping behavior in pure methods; keep Bannerlord object mutation in the behavior layer
- **Readable constants:** Economic tuning values should stay as named constants, not inline literals scattered through methods
- **Player documentation parity:** If gameplay behavior changes, update `README.md` so the Player Guide and Settings sections stay accurate

## Module Metadata

`Module/SubModule.xml` defines the Bannerlord module:

- **Name:** `True Town Gold`
- **Id:** `TrueTownGold`
- **Dependencies:** `Native`, `SandBoxCore`, `Sandbox`, `StoryMode`
- **Entry point:** `TrueTownGold.SubModule`

Keep `Module/SubModule.xml` aligned with any assembly, namespace, or entry point changes.

## Post-Change Workflow

After making any code changes, follow this order:

1. **Build:** `dotnet build src\TrueTownGold\TrueTownGold.csproj -c Release`
2. **Write or update tests:** Cover changed calculator behavior, guard logic, or structural integration points
3. **Test:** `dotnet test tests\TrueTownGold.Tests\TrueTownGold.Tests.csproj`
4. **Deploy:** `.\deploy.ps1`

Do not deploy if the build fails or tests are failing.

## Testing Guidelines

- Tests rely on `InternalsVisibleTo` so they can call `internal` calculator and behavior methods directly
- Prefer direct unit tests for `TownGoldCalculator` with explicit prosperity and gold inputs, especially around minimum, maximum, invalid, and boundary cases
- For `TrueTownGoldBehavior`, favor structural and guard-path tests that do not require a live campaign runtime
- Keep tests focused: one behavior per test, with clear expected outcomes for target-gold math and no-op scenarios
- Full integration tests that validate actual in-game town gold changes require a live Bannerlord campaign context and are outside normal unit test coverage
