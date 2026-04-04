using System;

namespace TrueTownGold
{
    /// <summary>
    /// Calculates the minimum gold that trade towns should have available based on prosperity.
    /// </summary>
    internal static class TownGoldCalculator
    {
        internal const float GoldPerProsperity = 10f;
        internal const int MinimumTownGold = 15000;
        internal const int MaximumTownGold = 150000;

        internal static int CalculateTargetGold(float prosperity)
        {
            if (float.IsNaN(prosperity) || float.IsInfinity(prosperity) || prosperity <= 0f)
            {
                return MinimumTownGold;
            }

            double calculatedGold = Math.Round(
                prosperity * GoldPerProsperity,
                MidpointRounding.AwayFromZero);

            int targetGold = calculatedGold >= int.MaxValue
                ? int.MaxValue
                : (int)calculatedGold;

            if (targetGold < MinimumTownGold)
            {
                return MinimumTownGold;
            }

            if (targetGold > MaximumTownGold)
            {
                return MaximumTownGold;
            }

            return targetGold;
        }

        internal static int CalculateRequiredGoldIncrease(float prosperity, int currentGold)
        {
            int targetGold = CalculateTargetGold(prosperity);
            int normalizedCurrentGold = Math.Max(0, currentGold);

            return normalizedCurrentGold >= targetGold
                ? 0
                : targetGold - normalizedCurrentGold;
        }
    }
}