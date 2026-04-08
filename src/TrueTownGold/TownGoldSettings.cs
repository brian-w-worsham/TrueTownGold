using System;
using System.Globalization;
using System.IO;
using System.Xml.Linq;

namespace TrueTownGold
{
    /// <summary>
    /// Loads the editable module settings that tune town gold generation.
    /// </summary>
    internal sealed class TownGoldSettings
    {
        internal const string SettingsFileName = "TrueTownGold.settings.xml";
        internal const float LegacyDefaultGlobalTownGoldMultiplier = 2.0f;
        internal const int LegacyDefaultMinimumTownGold = 15000;
        internal const int LegacyDefaultMaximumTownGold = 500000;
        internal const float DefaultGlobalTownGoldMultiplier = 10.0f;
        internal const int DefaultMinimumTownGold = 100000;
        internal const int DefaultMaximumTownGold = 999999999;

        internal static TownGoldSettings Current { get; private set; } = new TownGoldSettings();
        internal static bool LastLoadMigratedLegacyDefaults { get; private set; }

        public float GlobalTownGoldMultiplier { get; set; } = DefaultGlobalTownGoldMultiplier;
        public int MinimumTownGold { get; set; } = DefaultMinimumTownGold;
        public int MaximumTownGold { get; set; } = DefaultMaximumTownGold;

        internal static TownGoldSettings LoadFromDefaultPath()
        {
            string settingsFilePath = GetDefaultSettingsFilePath();
            LastLoadMigratedLegacyDefaults = false;
            Current = Load(settingsFilePath);

            if (ShouldReplaceLegacyDefaults(Current))
            {
                Current = new TownGoldSettings();
                LastLoadMigratedLegacyDefaults = true;
                TrySave(settingsFilePath, Current);
            }

            return Current;
        }

        internal static TownGoldSettings Load(string settingsFilePath)
        {
            if (string.IsNullOrWhiteSpace(settingsFilePath))
            {
                return new TownGoldSettings();
            }

            try
            {
                if (!File.Exists(settingsFilePath))
                {
                    return new TownGoldSettings();
                }

                XDocument document = XDocument.Load(settingsFilePath);
                string multiplierValue = document.Root?.Element(nameof(GlobalTownGoldMultiplier))?.Value;
                string minimumTownGoldValue = document.Root?.Element(nameof(MinimumTownGold))?.Value;
                string maximumTownGoldValue = document.Root?.Element(nameof(MaximumTownGold))?.Value;
                int minimumTownGold = ParseMinimumTownGold(minimumTownGoldValue);

                return new TownGoldSettings
                {
                    GlobalTownGoldMultiplier = ParseGlobalTownGoldMultiplier(multiplierValue),
                    MinimumTownGold = minimumTownGold,
                    MaximumTownGold = ParseMaximumTownGold(maximumTownGoldValue, minimumTownGold)
                };
            }
            catch
            {
                return new TownGoldSettings();
            }
        }

        internal static string GetDefaultSettingsFilePath()
        {
            string assemblyDirectory = Path.GetDirectoryName(typeof(SubModule).Assembly.Location);

            if (string.IsNullOrWhiteSpace(assemblyDirectory))
            {
                assemblyDirectory = AppDomain.CurrentDomain.BaseDirectory;
            }

            string moduleDirectory = Path.GetFullPath(Path.Combine(assemblyDirectory, "..", ".."));

            return Path.Combine(moduleDirectory, SettingsFileName);
        }

        internal float GetValidatedGlobalTownGoldMultiplier()
        {
            return ValidateGlobalTownGoldMultiplier(GlobalTownGoldMultiplier);
        }

        internal int GetValidatedMinimumTownGold()
        {
            return ValidateMinimumTownGold(MinimumTownGold);
        }

        internal int GetValidatedMaximumTownGold()
        {
            return ValidateMaximumTownGold(MaximumTownGold, GetValidatedMinimumTownGold());
        }

        internal static float ValidateGlobalTownGoldMultiplier(float multiplier)
        {
            return float.IsNaN(multiplier) || float.IsInfinity(multiplier) || multiplier <= 0f
                ? DefaultGlobalTownGoldMultiplier
                : multiplier;
        }

        internal static int ValidateMinimumTownGold(int minimumTownGold)
        {
            return minimumTownGold <= 0
                ? DefaultMinimumTownGold
                : minimumTownGold;
        }

        internal static int ValidateMaximumTownGold(int maximumTownGold, int minimumTownGold)
        {
            int validatedMaximumTownGold = maximumTownGold <= 0
                ? DefaultMaximumTownGold
                : maximumTownGold;

            return validatedMaximumTownGold < minimumTownGold
                ? minimumTownGold
                : validatedMaximumTownGold;
        }

        internal static bool ShouldReplaceLegacyDefaults(TownGoldSettings settings)
        {
            if (settings == null)
            {
                return false;
            }

            return Math.Abs(settings.GlobalTownGoldMultiplier - LegacyDefaultGlobalTownGoldMultiplier) < 0.0001f
                && settings.MinimumTownGold == LegacyDefaultMinimumTownGold
                && settings.MaximumTownGold == LegacyDefaultMaximumTownGold;
        }

        private static float ParseGlobalTownGoldMultiplier(string multiplierValue)
        {
            if (!float.TryParse(
                multiplierValue,
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out float parsedMultiplier))
            {
                return DefaultGlobalTownGoldMultiplier;
            }

            return ValidateGlobalTownGoldMultiplier(parsedMultiplier);
        }

        private static int ParseMinimumTownGold(string minimumTownGoldValue)
        {
            if (!int.TryParse(
                minimumTownGoldValue,
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out int parsedMinimumTownGold))
            {
                return DefaultMinimumTownGold;
            }

            return ValidateMinimumTownGold(parsedMinimumTownGold);
        }

        private static int ParseMaximumTownGold(string maximumTownGoldValue, int minimumTownGold)
        {
            if (!int.TryParse(
                maximumTownGoldValue,
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out int parsedMaximumTownGold))
            {
                return ValidateMaximumTownGold(DefaultMaximumTownGold, minimumTownGold);
            }

            return ValidateMaximumTownGold(parsedMaximumTownGold, minimumTownGold);
        }

        private static void TrySave(string settingsFilePath, TownGoldSettings settings)
        {
            if (string.IsNullOrWhiteSpace(settingsFilePath) || settings == null)
            {
                return;
            }

            try
            {
                string directory = Path.GetDirectoryName(settingsFilePath);
                if (!string.IsNullOrWhiteSpace(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var document = new XDocument(
                    new XElement(
                        "TrueTownGoldSettings",
                        new XElement(
                            nameof(GlobalTownGoldMultiplier),
                            settings.GetValidatedGlobalTownGoldMultiplier().ToString(CultureInfo.InvariantCulture)),
                        new XElement(nameof(MinimumTownGold), settings.GetValidatedMinimumTownGold()),
                        new XElement(nameof(MaximumTownGold), settings.GetValidatedMaximumTownGold())));

                document.Save(settingsFilePath);
            }
            catch
            {
                // Keep running with the migrated in-memory settings even if the file cannot be rewritten.
            }
        }
    }
}