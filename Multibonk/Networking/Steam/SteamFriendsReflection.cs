using System;
using System.Linq;
using System.Reflection;
using MelonLoader;

namespace Multibonk.Networking.Steam
{
    internal static class SteamFriendsReflection
    {
        private static readonly string[] CandidateAssemblies =
        {
            "com.rlabrecque.steamworks.net",
            "Steamworks.NET",
            "Assembly-CSharp-firstpass",
            "Assembly-CSharp"
        };

        public static Type? LocateSteamFriendsType()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = FindSteamFriendsType(assembly);
                if (type != null)
                {
                    return type;
                }
            }

            foreach (var assemblyName in CandidateAssemblies)
            {
                var qualifiedName = $"Steamworks.SteamFriends, {assemblyName}";
                try
                {
                    var type = Type.GetType(qualifiedName, throwOnError: false);
                    if (type != null)
                    {
                        return type;
                    }
                }
                catch (ReflectionTypeLoadException ex)
                {
                    MelonLogger.Warning($"Failed to inspect '{qualifiedName}': {ex.Message}");
                }
            }

            return null;
        }

        public static MethodInfo? FindActivateGameOverlay(Type steamFriendsType)
        {
            try
            {
                return steamFriendsType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .FirstOrDefault(method =>
                        method.Name == "ActivateGameOverlay" &&
                        method.GetParameters().Length == 1 &&
                        method.GetParameters()[0].ParameterType == typeof(string));
            }
            catch (ReflectionTypeLoadException ex)
            {
                MelonLogger.Warning($"Could not enumerate types from '{steamFriendsType.Assembly.FullName}': {ex.Message}");
                return null;
            }
        }

        private static Type? FindSteamFriendsType(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes().FirstOrDefault(type => type.FullName == "Steamworks.SteamFriends");
            }
            catch (ReflectionTypeLoadException ex)
            {
                MelonLogger.Warning($"Could not enumerate types from '{assembly.FullName}': {ex.Message}");
                return null;
            }
        }
    }
}
