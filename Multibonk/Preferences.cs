using MelonLoader;

namespace Multibonk
{
    public static class Preferences
    {
        private static readonly MelonPreferences_Category category;

        public static readonly MelonPreferences_Entry<string> IpAddress;
        public static readonly MelonPreferences_Entry<string> PlayerName;

        public static readonly MelonPreferences_Entry<bool> LevelSynchronization;
        public static readonly MelonPreferences_Entry<bool> PauseSynchronization;


        static Preferences()
        {
            category = MelonPreferences.CreateCategory("Multibonk", "General Settings");

            IpAddress = category.CreateEntry("IpAddress", "127.0.0.1", description: "IP address used at connection window");
            PlayerName = category.CreateEntry("PlayerName", "PlayerName", description: "PlayerName used at connection window");

            LevelSynchronization = category.CreateEntry("LevelSynchronization", true, description: "Enable or disable sharing levels and XP");
            PauseSynchronization = category.CreateEntry("PauseSynchronization", true, description: "When enabled, your game will automatically pause whenever a friend pauses theirs.");
        }
    }
}
