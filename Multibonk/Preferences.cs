using MelonLoader;

namespace Multibonk
{
    public static class Preferences
    {
        private static readonly MelonPreferences_Category category;

        public static readonly MelonPreferences_Entry<string> IpAddress;
        public static readonly MelonPreferences_Entry<string> PlayerName;

        static Preferences()
        {
            category = MelonPreferences.CreateCategory("Multibonk", "General Settings");

            IpAddress = category.CreateEntry("IpAddress", "127.0.0.1", description: "IP address used at connection window");
            PlayerName = category.CreateEntry("PlayerName", "PlayerName", description: "PlayerName used at connection window");
        }
    }
}
