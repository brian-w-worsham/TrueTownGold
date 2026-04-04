# True Town Gold — Bannerlord Mod

Need larger town purses for trading and post-battle loot sales? This mod gives every trade town a prosperity-based minimum gold reserve, so richer settlements can buy more of your goods without forcing you to bounce across the map.

## Player Guide

### What the mod does for players

Once enabled, the mod automatically increases the minimum gold available in trade towns based on each town's prosperity. You do not need to press a hotkey, open a menu, or trigger anything manually. The effect is applied automatically during campaign play.

### How to install and enable it

If you are using this repository directly:

1. Build and deploy the mod with `.\deploy.ps1`.
2. Confirm the folder `Mount & Blade II Bannerlord\Modules\TrueTownGold` exists.
3. Confirm `SubModule.xml` is in that module folder.
4. Confirm `TrueTownGold.dll` is in `Modules\TrueTownGold\bin\Win64_Shipping_Client`.
5. Launch the Bannerlord launcher.
6. Open the **Mods** tab.
7. Enable **True Town Gold**.
8. Start or load a single-player campaign.

If you are installing from a packaged release instead, place the `TrueTownGold` folder inside your Bannerlord `Modules` directory and then enable it in the launcher.

### How to use the mod in-game

1. Load into a campaign save.
2. Wait at least one in-game day so the daily town update runs.
3. Visit a town and open the trade screen.
4. Sell loot, armor, weapons, horses, or trade goods as usual.
5. Richer towns should be able to buy noticeably more before running out of gold.

### What to expect

- Town gold is adjusted automatically once per in-game day.
- High-prosperity towns receive a larger gold floor than poor towns.
- The mod only raises towns that are below the target amount.
- The mod does not add a UI, hotkeys, or extra player actions.

### Settings

There are currently no player-adjustable settings.

- No MCM menu
- No XML or JSON config file
- No in-game toggle
- No console commands

The current built-in values are:

- `GoldPerProsperity = 10`
- `MinimumTownGold = 15,000`
- `MaximumTownGold = 150,000`

If you want different values, the mod would need a code change and rebuild.

## Features

- **Prosperity-based town gold floor:** Each day, every trade town is topped up to a minimum gold amount derived from its prosperity
- **Standalone implementation:** No external controller or MCM dependency is required
- **Low-risk economy change:** The mod only adds missing gold up to the target floor; it does not force towns back down if they already have more
- **Save-safe behavior:** The feature is applied through a campaign behavior and does not add persistent save data

## How It Works

The mod listens to Bannerlord's `DailyTickTownEvent`. For each real trade town, it calculates a target reserve using this formula:

```text
targetGold = clamp(prosperity × 10, 15,000, 150,000)
```

If a town already has at least that much gold, nothing happens. If it has less, the difference is added with `Town.ChangeGold(...)`.

This keeps wealthy towns meaningfully liquid while avoiding a more invasive full reset of the economy.

## Prerequisites

- **Mount & Blade II: Bannerlord** installed locally
- **.NET Framework 4.7.2** targeting pack
- **Visual Studio 2022** or a compatible **.NET SDK**
- Bannerlord installed at the expected path, or update the `GameFolder` property in the project files

## Project Structure

```text
TrueTownGold/
├── TrueTownGold.sln
├── deploy.ps1
├── README.md
├── Module/
│   └── SubModule.xml
├── src/
│   └── TrueTownGold/
│       ├── TrueTownGold.csproj
│       ├── SubModule.cs
│       ├── TownGoldCalculator.cs
│       └── TrueTownGoldBehavior.cs
└── tests/
    └── TrueTownGold.Tests/
        ├── TrueTownGold.Tests.csproj
        ├── TownGoldCalculatorTests.cs
        └── TrueTownGoldBehaviorTests.cs
```

## Setup & Build

### 1. Verify your game path

Check `src/TrueTownGold/TrueTownGold.csproj` and confirm the `GameFolder` value points to your Bannerlord installation.

### 2. Build

```powershell
dotnet build src\TrueTownGold\TrueTownGold.csproj -c Release
```

### 3. Run tests

```powershell
dotnet test tests\TrueTownGold.Tests\TrueTownGold.Tests.csproj
```

### 4. Deploy

```powershell
.\deploy.ps1
```

You can also supply a custom game path:

```powershell
.\deploy.ps1 -GameFolder "D:\Steam\steamapps\common\Mount & Blade II Bannerlord"
```

## In-Game Verification

1. Launch Bannerlord.
2. Enable **True Town Gold** in the launcher.
3. Start or load a campaign save.
4. Wait at least one in-game day so the daily town tick runs.
5. Travel to a prosperous city such as Marunath, Epicrotea, or Sanala.
6. Open trade and sell expensive loot, armor, or horses.
7. Confirm that the town can afford significantly larger sales than vanilla and does not run out of gold as quickly.
8. Compare a low-prosperity town against a high-prosperity town to verify that richer settlements generally have deeper buying power.
