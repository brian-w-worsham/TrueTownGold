using System;
using System.IO;
using Xunit;

namespace TrueTownGold.Tests
{
    public class TownGoldSettingsTests
    {
        [Fact]
        public void Load_MissingFile_ReturnsDefaultSettings()
        {
            string settingsPath = Path.Combine(
                Path.GetTempPath(),
                Guid.NewGuid().ToString("N"),
                TownGoldSettings.SettingsFileName);

            TownGoldSettings settings = TownGoldSettings.Load(settingsPath);

            Assert.Equal(
                TownGoldSettings.DefaultGlobalTownGoldMultiplier,
                settings.GlobalTownGoldMultiplier);
            Assert.Equal(TownGoldSettings.DefaultMinimumTownGold, settings.MinimumTownGold);
            Assert.Equal(TownGoldSettings.DefaultMaximumTownGold, settings.MaximumTownGold);
        }

        [Fact]
        public void ShouldReplaceLegacyDefaults_LegacyDefaults_ReturnsTrue()
        {
            var settings = new TownGoldSettings
            {
                GlobalTownGoldMultiplier = TownGoldSettings.LegacyDefaultGlobalTownGoldMultiplier,
                MinimumTownGold = TownGoldSettings.LegacyDefaultMinimumTownGold,
                MaximumTownGold = TownGoldSettings.LegacyDefaultMaximumTownGold
            };

            Assert.True(TownGoldSettings.ShouldReplaceLegacyDefaults(settings));
        }

        [Fact]
        public void ShouldReplaceLegacyDefaults_CustomSettings_ReturnsFalse()
        {
            var settings = new TownGoldSettings
            {
                GlobalTownGoldMultiplier = 6.5f,
                MinimumTownGold = 90000,
                MaximumTownGold = 750000
            };

            Assert.False(TownGoldSettings.ShouldReplaceLegacyDefaults(settings));
        }

        [Fact]
        public void Load_ValidFile_ReadsConfiguredValues()
        {
            string tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDirectory);
            string settingsPath = Path.Combine(tempDirectory, TownGoldSettings.SettingsFileName);

            try
            {
                File.WriteAllText(
                    settingsPath,
                    "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" +
                    "<TrueTownGoldSettings>\r\n" +
                    "  <GlobalTownGoldMultiplier>3.5</GlobalTownGoldMultiplier>\r\n" +
                    "  <MinimumTownGold>22000</MinimumTownGold>\r\n" +
                    "  <MaximumTownGold>640000</MaximumTownGold>\r\n" +
                    "</TrueTownGoldSettings>\r\n");

                TownGoldSettings settings = TownGoldSettings.Load(settingsPath);

                Assert.Equal(3.5f, settings.GlobalTownGoldMultiplier);
                Assert.Equal(22000, settings.MinimumTownGold);
                Assert.Equal(640000, settings.MaximumTownGold);
            }
            finally
            {
                if (Directory.Exists(tempDirectory))
                {
                    Directory.Delete(tempDirectory, true);
                }
            }
        }

        [Fact]
        public void ValidateGlobalTownGoldMultiplier_InvalidValue_ReturnsDefault()
        {
            Assert.Equal(
                TownGoldSettings.DefaultGlobalTownGoldMultiplier,
                TownGoldSettings.ValidateGlobalTownGoldMultiplier(0f));
            Assert.Equal(
                TownGoldSettings.DefaultGlobalTownGoldMultiplier,
                TownGoldSettings.ValidateGlobalTownGoldMultiplier(float.NaN));
            Assert.Equal(
                TownGoldSettings.DefaultGlobalTownGoldMultiplier,
                TownGoldSettings.ValidateGlobalTownGoldMultiplier(float.PositiveInfinity));
        }

        [Fact]
        public void ValidateMinimumTownGold_InvalidValue_ReturnsDefault()
        {
            Assert.Equal(
                TownGoldSettings.DefaultMinimumTownGold,
                TownGoldSettings.ValidateMinimumTownGold(0));
            Assert.Equal(
                TownGoldSettings.DefaultMinimumTownGold,
                TownGoldSettings.ValidateMinimumTownGold(-100));
        }

        [Fact]
        public void ValidateMaximumTownGold_BelowMinimum_ReturnsMinimum()
        {
            Assert.Equal(
                25000,
                TownGoldSettings.ValidateMaximumTownGold(10000, 25000));
        }
    }
}