using System;

namespace TrueTownGold
{
    /// <summary>
    /// Calculates the minimum gold that trade towns should have available based on prosperity.
    /// </summary>
    internal static class TownGoldCalculator
    {
        internal const float BaseGoldPerProsperity = 10f;
        internal const float DefaultGoldPerProsperity =
            BaseGoldPerProsperity * TownGoldSettings.DefaultGlobalTownGoldMultiplier;
        internal const int DefaultMinimumTownGold = TownGoldSettings.DefaultMinimumTownGold;
        internal const int DefaultMaximumTownGold = TownGoldSettings.DefaultMaximumTownGold;

        internal static int CalculateTargetGold(float prosperity)
        {
            return CalculateTargetGold(
                prosperity,
                TownGoldSettings.Current.GlobalTownGoldMultiplier,
                TownGoldSettings.Current.MinimumTownGold,
                TownGoldSettings.Current.MaximumTownGold);
        }

        internal static int CalculateTargetGold(float prosperity, float globalTownGoldMultiplier)
        {
            return CalculateTargetGold(
                prosperity,
                globalTownGoldMultiplier,
                DefaultMinimumTownGold,
                DefaultMaximumTownGold);
        }

        internal static int CalculateTargetGold(
            float prosperity,
            float globalTownGoldMultiplier,
            int minimumTownGold,
            int maximumTownGold)
        {
            int validatedMinimumTownGold = TownGoldSettings.ValidateMinimumTownGold(minimumTownGold);
            int validatedMaximumTownGold =
                TownGoldSettings.ValidateMaximumTownGold(maximumTownGold, validatedMinimumTownGold);

            if (float.IsNaN(prosperity) || float.IsInfinity(prosperity) || prosperity <= 0f)
            {
                return validatedMinimumTownGold;
            }

            float goldPerProsperity = GetGoldPerProsperity(globalTownGoldMultiplier);

            double calculatedGold = Math.Round(
                prosperity * goldPerProsperity,
                MidpointRounding.AwayFromZero);

            int targetGold = calculatedGold >= int.MaxValue
                ? int.MaxValue
                : (int)calculatedGold;

            if (targetGold < validatedMinimumTownGold)
            {
                return validatedMinimumTownGold;
            }

            if (targetGold > validatedMaximumTownGold)
            {
                return validatedMaximumTownGold;
            }

            return targetGold;
        }

        internal static int CalculateRequiredGoldIncrease(float prosperity, int currentGold)
        {
            return CalculateRequiredGoldIncrease(
                prosperity,
                currentGold,
                TownGoldSettings.Current.GlobalTownGoldMultiplier,
                TownGoldSettings.Current.MinimumTownGold,
                TownGoldSettings.Current.MaximumTownGold);
        }

        internal static int CalculateRequiredGoldIncrease(
            float prosperity,
            int currentGold,
            float globalTownGoldMultiplier)
        {
            return CalculateRequiredGoldIncrease(
                prosperity,
                currentGold,
                globalTownGoldMultiplier,
                DefaultMinimumTownGold,
                DefaultMaximumTownGold);
        }

        internal static int CalculateRequiredGoldIncrease(
            float prosperity,
            int currentGold,
            float globalTownGoldMultiplier,
            int minimumTownGold,
            int maximumTownGold)
        {
            int targetGold = CalculateTargetGold(
                prosperity,
                globalTownGoldMultiplier,
                minimumTownGold,
                maximumTownGold);
            int normalizedCurrentGold = Math.Max(0, currentGold);

            return normalizedCurrentGold >= targetGold
                ? 0
                : targetGold - normalizedCurrentGold;
        }

        internal static float GetGoldPerProsperity(float globalTownGoldMultiplier)
        {
            return BaseGoldPerProsperity *
                TownGoldSettings.ValidateGlobalTownGoldMultiplier(globalTownGoldMultiplier);
        }
    }
}