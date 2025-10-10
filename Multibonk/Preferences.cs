using System;
using MelonLoader;

namespace Multibonk
{
    public static class Preferences
    {
        public enum LootDistributionMode
        {
            Shared,
            Individual,
            Duplicated
        }

        private static readonly MelonPreferences_Category category;

        public static readonly MelonPreferences_Entry<string> PlayerName;
        public static readonly MelonPreferences_Entry<bool> PvpEnabled;
        public static readonly MelonPreferences_Entry<bool> ReviveEnabled;
        public static readonly MelonPreferences_Entry<float> ReviveTimeSeconds;
        public static readonly MelonPreferences_Entry<string> XpSharingMode;
        public static readonly MelonPreferences_Entry<string> GoldSharingMode;
        public static readonly MelonPreferences_Entry<string> ChestSharingMode;

        static Preferences()
        {
            category = MelonPreferences.CreateCategory("Multibonk", "General Settings");

            PlayerName = category.CreateEntry("PlayerName", "PlayerName", description: "Default player name used when connecting to a lobby.");

            PvpEnabled = category.CreateEntry("PvpEnabled", false, description: "Enable player-versus-player damage in multiplayer sessions.");
            ReviveEnabled = category.CreateEntry("ReviveEnabled", true, description: "Allow players to revive fallen teammates.");
            ReviveTimeSeconds = category.CreateEntry("ReviveTimeSeconds", 5f, description: "Delay, in seconds, before a downed player can be revived.");

            XpSharingMode = category.CreateEntry("XpSharingMode", LootDistributionMode.Shared.ToString(), description: "How experience orbs are distributed between players.");
            GoldSharingMode = category.CreateEntry("GoldSharingMode", LootDistributionMode.Shared.ToString(), description: "How collected gold is distributed between players.");
            ChestSharingMode = category.CreateEntry("ChestSharingMode", LootDistributionMode.Shared.ToString(), description: "How chest rewards are distributed between players.");
        }

        public static LootDistributionMode GetXpSharingMode() => ParseEnumEntry(XpSharingMode, LootDistributionMode.Shared);

        public static void SetXpSharingMode(LootDistributionMode mode) => XpSharingMode.Value = mode.ToString();

        public static LootDistributionMode GetGoldSharingMode() => ParseEnumEntry(GoldSharingMode, LootDistributionMode.Shared);

        public static void SetGoldSharingMode(LootDistributionMode mode) => GoldSharingMode.Value = mode.ToString();

        public static LootDistributionMode GetChestSharingMode() => ParseEnumEntry(ChestSharingMode, LootDistributionMode.Shared);

        public static void SetChestSharingMode(LootDistributionMode mode) => ChestSharingMode.Value = mode.ToString();

        private static LootDistributionMode ParseEnumEntry(MelonPreferences_Entry<string> entry, LootDistributionMode fallback)
        {
            if (Enum.TryParse(entry.Value, out LootDistributionMode value))
            {
                return value;
            }

            entry.Value = fallback.ToString();
            return fallback;
        }
    }
}
