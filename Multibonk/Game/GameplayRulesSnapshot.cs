using System;

namespace Multibonk.Game
{
    public readonly struct GameplayRulesSnapshot : IEquatable<GameplayRulesSnapshot>
    {
        public bool PvpEnabled { get; }
        public bool ReviveEnabled { get; }
        public float ReviveDelaySeconds { get; }
        public Preferences.LootDistributionMode XpSharingMode { get; }
        public Preferences.LootDistributionMode GoldSharingMode { get; }
        public Preferences.LootDistributionMode ChestSharingMode { get; }

        public GameplayRulesSnapshot(
            bool pvpEnabled,
            bool reviveEnabled,
            float reviveDelaySeconds,
            Preferences.LootDistributionMode xpSharingMode,
            Preferences.LootDistributionMode goldSharingMode,
            Preferences.LootDistributionMode chestSharingMode)
        {
            PvpEnabled = pvpEnabled;
            ReviveEnabled = reviveEnabled;
            ReviveDelaySeconds = reviveDelaySeconds;
            XpSharingMode = xpSharingMode;
            GoldSharingMode = goldSharingMode;
            ChestSharingMode = chestSharingMode;
        }

        public static GameplayRulesSnapshot FromPreferences() => new GameplayRulesSnapshot(
            Preferences.PvpEnabled.Value,
            Preferences.ReviveEnabled.Value,
            Preferences.ReviveTimeSeconds.Value,
            Preferences.GetXpSharingMode(),
            Preferences.GetGoldSharingMode(),
            Preferences.GetChestSharingMode());

        public bool Equals(GameplayRulesSnapshot other) =>
            PvpEnabled == other.PvpEnabled &&
            ReviveEnabled == other.ReviveEnabled &&
            Math.Abs(ReviveDelaySeconds - other.ReviveDelaySeconds) < 0.001f &&
            XpSharingMode == other.XpSharingMode &&
            GoldSharingMode == other.GoldSharingMode &&
            ChestSharingMode == other.ChestSharingMode;

        public override bool Equals(object? obj) => obj is GameplayRulesSnapshot other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(PvpEnabled, ReviveEnabled, ReviveDelaySeconds, XpSharingMode, GoldSharingMode, ChestSharingMode);

        public override string ToString() =>
            $"PvP={(PvpEnabled ? "Enabled" : "Disabled")}, Revive={(ReviveEnabled ? "Enabled" : "Disabled")} ({ReviveDelaySeconds:0.##}s), XP={XpSharingMode}, Gold={GoldSharingMode}, Chest={ChestSharingMode}";
    }
}
