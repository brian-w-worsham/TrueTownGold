using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;

namespace TrueTownGold
{
    /// <summary>
    /// Campaign behavior that ensures each trade town has a prosperity-based minimum gold reserve.
    /// </summary>
    public class TrueTownGoldBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickTownEvent.AddNonSerializedListener(this, OnDailyTickTown);
        }

        public override void SyncData(IDataStore dataStore)
        {
        }

        internal static void OnDailyTickTown(Town town)
        {
            EnsureTownHasTradeGold(town);
        }

        internal static bool EnsureTownHasTradeGold(Town town)
        {
            if (town?.Settlement == null || !town.Settlement.IsTown)
            {
                return false;
            }

            int requiredGoldIncrease = TownGoldCalculator.CalculateRequiredGoldIncrease(
                town.Prosperity,
                town.Gold);

            if (requiredGoldIncrease <= 0)
            {
                return false;
            }

            town.ChangeGold(requiredGoldIncrease);
            return true;
        }
    }
}