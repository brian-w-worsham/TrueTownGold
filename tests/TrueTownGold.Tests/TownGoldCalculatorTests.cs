using Xunit;

namespace TrueTownGold.Tests
{
    public class TownGoldCalculatorTests
    {
        [Fact]
        public void CalculateTargetGold_NonPositiveProsperity_ReturnsMinimumTownGold()
        {
            Assert.Equal(TownGoldCalculator.MinimumTownGold, TownGoldCalculator.CalculateTargetGold(0f));
            Assert.Equal(TownGoldCalculator.MinimumTownGold, TownGoldCalculator.CalculateTargetGold(-10f));
        }

        [Fact]
        public void CalculateTargetGold_InvalidProsperity_ReturnsMinimumTownGold()
        {
            Assert.Equal(TownGoldCalculator.MinimumTownGold, TownGoldCalculator.CalculateTargetGold(float.NaN));
            Assert.Equal(TownGoldCalculator.MinimumTownGold, TownGoldCalculator.CalculateTargetGold(float.PositiveInfinity));
        }

        [Fact]
        public void CalculateTargetGold_LowProsperity_StillReturnsMinimumTownGold()
        {
            int targetGold = TownGoldCalculator.CalculateTargetGold(1200f);

            Assert.Equal(TownGoldCalculator.MinimumTownGold, targetGold);
        }

        [Fact]
        public void CalculateTargetGold_ProsperityAtMinimumBoundary_ReturnsBoundaryValue()
        {
            int targetGold = TownGoldCalculator.CalculateTargetGold(1500f);

            Assert.Equal(15000, targetGold);
        }

        [Fact]
        public void CalculateTargetGold_UsesProsperityMultiplierAndRoundsAwayFromZero()
        {
            int targetGold = TownGoldCalculator.CalculateTargetGold(2750.5f);

            Assert.Equal(27505, targetGold);
        }

        [Fact]
        public void CalculateTargetGold_HighProsperity_IsClampedToMaximumTownGold()
        {
            int targetGold = TownGoldCalculator.CalculateTargetGold(50000f);

            Assert.Equal(TownGoldCalculator.MaximumTownGold, targetGold);
        }

        [Fact]
        public void CalculateRequiredGoldIncrease_CurrentGoldBelowTarget_ReturnsMissingAmount()
        {
            int requiredGoldIncrease = TownGoldCalculator.CalculateRequiredGoldIncrease(5000f, 32000);

            Assert.Equal(18000, requiredGoldIncrease);
        }

        [Fact]
        public void CalculateRequiredGoldIncrease_CurrentGoldAtTarget_ReturnsZero()
        {
            int requiredGoldIncrease = TownGoldCalculator.CalculateRequiredGoldIncrease(5000f, 50000);

            Assert.Equal(0, requiredGoldIncrease);
        }

        [Fact]
        public void CalculateRequiredGoldIncrease_CurrentGoldAboveTarget_ReturnsZero()
        {
            int requiredGoldIncrease = TownGoldCalculator.CalculateRequiredGoldIncrease(5000f, 65000);

            Assert.Equal(0, requiredGoldIncrease);
        }

        [Fact]
        public void CalculateRequiredGoldIncrease_NegativeCurrentGold_TreatsItAsZero()
        {
            int requiredGoldIncrease = TownGoldCalculator.CalculateRequiredGoldIncrease(2000f, -50);

            Assert.Equal(20000, requiredGoldIncrease);
        }
    }
}