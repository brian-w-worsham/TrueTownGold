using System;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace TrueTownGold
{
    /// <summary>
    /// Entry point for the TrueTownGold mod. Registers a campaign behavior that
    /// keeps trade towns supplied with prosperity-scaled gold for barter and loot sales.
    /// </summary>
    public class SubModule : MBSubModuleBase
    {
        private Harmony _harmony;

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            try
            {
                _harmony = new Harmony("com.truetowngold.bannerlord");
                _harmony.PatchAll();
                InformationManager.DisplayMessage(
                    new InformationMessage("True Town Gold: Loaded successfully.", Colors.Green));
            }
            catch (Exception ex)
            {
                InformationManager.DisplayMessage(
                    new InformationMessage($"True Town Gold load error: {ex.Message}", Colors.Red));
            }
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);
            if (game.GameType is Campaign)
            {
                CampaignGameStarter campaignStarter = (CampaignGameStarter)gameStarterObject;
                campaignStarter.AddBehavior(new TrueTownGoldBehavior());
            }
        }

        protected override void OnSubModuleUnloaded()
        {
            base.OnSubModuleUnloaded();
            _harmony?.UnpatchAll("com.truetowngold.bannerlord");
        }
    }
}