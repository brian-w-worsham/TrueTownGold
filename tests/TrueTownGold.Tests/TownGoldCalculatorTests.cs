using Xunit;

namespace TrueTownGold.Tests
{
    public class TownGoldCalculatorTests
    {
        [Fact]
        public void CalculateTargetGold_NonPositiveProsperity_ReturnsMinimumTownGold()
        {
            Assert.Equal(TownGoldCalculator.DefaultMinimumTownGold, TownGoldCalculator.CalculateTargetGold(0f));
            Assert.Equal(TownGoldCalculator.DefaultMinimumTownGold, TownGoldCalculator.CalculateTargetGold(-10f));
        }

        [Fact]
        public void CalculateTargetGold_InvalidProsperity_ReturnsMinimumTownGold()
        {
            Assert.Equal(TownGoldCalculator.DefaultMinimumTownGold, TownGoldCalculator.CalculateTargetGold(float.NaN));
            Assert.Equal(TownGoldCalculator.DefaultMinimumTownGold, TownGoldCalculator.CalculateTargetGold(float.PositiveInfinity));
        }

        [Fact]
        public void CalculateTargetGold_LowProsperity_StillReturnsMinimumTownGold()
        {
            int targetGold = TownGoldCalculator.CalculateTargetGold(700f);

            Assert.Equal(TownGoldCalculator.DefaultMinimumTownGold, targetGold);
        }

        [Fact]
        public void CalculateTargetGold_ProsperityAtMinimumBoundary_ReturnsBoundaryValue()
        {
            int targetGold = TownGoldCalculator.CalculateTargetGold(
                1000f,
                TownGoldSettings.DefaultGlobalTownGoldMultiplier);

            Assert.Equal(TownGoldSettings.DefaultMinimumTownGold, targetGold);
        }

        [Fact]
        public void CalculateTargetGold_UsesConfiguredMultiplierAndRoundsAwayFromZero()
        {
            int targetGold = TownGoldCalculator.CalculateTargetGold(2750.5f, 2.0f, 15000, 500000);

            Assert.Equal(55010, targetGold);
        }

        [Fact]
        public void CalculateTargetGold_HighProsperity_IsClampedToMaximumTownGold()
        {
            int targetGold = TownGoldCalculator.CalculateTargetGold(20000000f);

            Assert.Equal(TownGoldCalculator.DefaultMaximumTownGold, targetGold);
        }

        [Fact]
        public void CalculateTargetGold_ConfiguredMaximum_AllowsExceedingPreviousCap()
        {
            int targetGold = TownGoldCalculator.CalculateTargetGold(12000f, 2.0f, 15000, 500000);

            Assert.Equal(240000, targetGold);
            Assert.True(targetGold > 150000);
        }

        [Fact]
        public void CalculateTargetGold_ConfiguredMinimum_IsApplied()
        {
            int targetGold = TownGoldCalculator.CalculateTargetGold(500f, 2.0f, 25000, 500000);

            Assert.Equal(25000, targetGold);
        }

        [Fact]
        public void CalculateRequiredGoldIncrease_CurrentGoldBelowTarget_ReturnsMissingAmount()
        {
            int requiredGoldIncrease = TownGoldCalculator.CalculateRequiredGoldIncrease(
                5000f,
                32000,
                2.0f);

            Assert.Equal(68000, requiredGoldIncrease);
        }

        [Fact]
        public void CalculateRequiredGoldIncrease_CurrentGoldAtTarget_ReturnsZero()
        {
            int requiredGoldIncrease = TownGoldCalculator.CalculateRequiredGoldIncrease(
                5000f,
                100000,
                2.0f);

            Assert.Equal(0, requiredGoldIncrease);
        }

        [Fact]
        public void CalculateRequiredGoldIncrease_CurrentGoldAboveTarget_ReturnsZero()
        {
            int requiredGoldIncrease = TownGoldCalculator.CalculateRequiredGoldIncrease(
                5000f,
                125000,
                2.0f);

            Assert.Equal(0, requiredGoldIncrease);
        }

        [Fact]
        public void CalculateRequiredGoldIncrease_NegativeCurrentGold_TreatsItAsZero()
        {
            int requiredGoldIncrease = TownGoldCalculator.CalculateRequiredGoldIncrease(
                2000f,
                -50,
                2.0f,
                15000,
                500000);

            Assert.Equal(40000, requiredGoldIncrease);
        }

        [Fact]
        public void GetGoldPerProsperity_InvalidMultiplier_UsesDefaultMultiplier()
        {
            float goldPerProsperity = TownGoldCalculator.GetGoldPerProsperity(0f);

            Assert.Equal(TownGoldCalculator.DefaultGoldPerProsperity, goldPerProsperity);
        }
    }
}