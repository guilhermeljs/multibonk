using System;
using System.Reflection;
using MelonLoader;
using Multibonk.Networking;

namespace Multibonk.Networking.Steam
{
    public class SteamTunnelCallbackBinder
    {
        private readonly SteamTunnelService steamTunnelService;
        private readonly object syncRoot = new();

        private Delegate? joinRequestedHandler;
        private EventInfo? joinRequestedEvent;
        private Type? subscribedSteamFriendsType;
        private bool missingSteamFriendsLogged;
        private bool missingEventLogged;

        public SteamTunnelCallbackBinder(SteamTunnelService steamTunnelService)
        {
            this.steamTunnelService = steamTunnelService;
            AppDomain.CurrentDomain.AssemblyLoad += HandleAssemblyLoaded;
            TryBindCallbacks();
        }

        public bool TryBindCallbacks()
        {
            lock (syncRoot)
            {
                var steamFriendsType = steamTunnelService.SteamFriendsType ?? SteamFriendsReflection.LocateSteamFriendsType();
                if (steamFriendsType == null)
                {
                    if (!missingSteamFriendsLogged)
                    {
                        MelonLogger.Warning("Steam friends API is unavailable; Steam join requests will be ignored until Steamworks initialises.");
                        missingSteamFriendsLogged = true;
                    }
                    return false;
                }

                if (subscribedSteamFriendsType == steamFriendsType && joinRequestedHandler != null)
                {
                    return true;
                }

                DetachHandlers_NoLock();

                joinRequestedEvent = steamFriendsType.GetEvent("OnGameRichPresenceJoinRequested", BindingFlags.Public | BindingFlags.Static);
                if (joinRequestedEvent == null || joinRequestedEvent.EventHandlerType == null)
                {
                    if (!missingEventLogged)
                    {
                        MelonLogger.Warning("SteamFriends.OnGameRichPresenceJoinRequested was not found; Steam invites cannot be consumed until the event becomes available.");
                        missingEventLogged = true;
                    }
                    return false;
                }

                var handlerMethod = typeof(SteamTunnelCallbackBinder).GetMethod(nameof(OnGameRichPresenceJoinRequested), BindingFlags.Instance | BindingFlags.NonPublic);
                if (handlerMethod == null)
                {
                    MelonLogger.Error("Steam tunnel callback binder lost its handler method.");
                    return false;
                }

                try
                {
                    joinRequestedHandler = Delegate.CreateDelegate(joinRequestedEvent.EventHandlerType, this, handlerMethod);
                    joinRequestedEvent.AddEventHandler(null, joinRequestedHandler);
                    subscribedSteamFriendsType = steamFriendsType;
                    MelonLogger.Msg($"Listening for Steam Rich Presence joins via '{steamFriendsType.Assembly.FullName}'.");
                    missingSteamFriendsLogged = false;
                    missingEventLogged = false;
                    return true;
                }
                catch (Exception ex)
                {
                    MelonLogger.Error($"Failed to subscribe to Steam join requests: {ex.Message}");
                    DetachHandlers_NoLock();
                    return false;
                }
            }
        }

        public void DetachHandlers()
        {
            lock (syncRoot)
            {
                DetachHandlers_NoLock();
            }
        }

        private void OnGameRichPresenceJoinRequested(object rawEvent)
        {
            if (rawEvent == null)
            {
                MelonLogger.Warning("Steam signalled a join request without payload; ignoring.");
                return;
            }

            if (!TryExtractConnectString(rawEvent, out var connectString))
            {
                MelonLogger.Warning($"Steam join request payload '{rawEvent.GetType().FullName}' did not expose a connect string.");
                return;
            }

            if (!TryParseConnectString(connectString, out var address, out var port))
            {
                MelonLogger.Warning($"Steam join request did not contain a valid endpoint: '{connectString}'.");
                return;
            }

            steamTunnelService.RegisterEndpoint(address, port);
        }

        private void DetachHandlers_NoLock()
        {
            if (joinRequestedHandler == null || joinRequestedEvent == null)
            {
                return;
            }

            try
            {
                joinRequestedEvent.RemoveEventHandler(null, joinRequestedHandler);
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"Failed to detach Steam join handler: {ex.Message}");
            }
            finally
            {
                joinRequestedHandler = null;
                joinRequestedEvent = null;
                subscribedSteamFriendsType = null;
            }
        }

        private void HandleAssemblyLoaded(object? sender, AssemblyLoadEventArgs args)
        {
            TryBindCallbacks();
        }

        private static bool TryExtractConnectString(object rawEvent, out string connectString)
        {
            var eventType = rawEvent.GetType();

            var field = eventType.GetField("m_rgchConnect", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (field?.GetValue(rawEvent) is string fieldValue && !string.IsNullOrWhiteSpace(fieldValue))
            {
                connectString = fieldValue.Trim();
                return true;
            }

            var property = eventType.GetProperty("Connect", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (property?.GetValue(rawEvent) is string propertyValue && !string.IsNullOrWhiteSpace(propertyValue))
            {
                connectString = propertyValue.Trim();
                return true;
            }

            var method = eventType.GetMethod("GetConnectString", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (method != null && method.GetParameters().Length == 0 && method.ReturnType == typeof(string))
            {
                if (method.Invoke(rawEvent, Array.Empty<object>()) is string methodValue && !string.IsNullOrWhiteSpace(methodValue))
                {
                    connectString = methodValue.Trim();
                    return true;
                }
            }

            connectString = string.Empty;
            return false;
        }

        private static bool TryParseConnectString(string connectString, out string address, out int port)
        {
            address = string.Empty;
            port = 0;

            var trimmed = connectString.Trim();
            if (trimmed.Length == 0)
            {
                return false;
            }

            var tokens = trimmed.Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < tokens.Length; i++)
            {
                var token = tokens[i];
                if (TryParseToken(token, out address, out port))
                {
                    return true;
                }

                if (IsConnectKeyword(token) && i + 1 < tokens.Length && TryParseToken(tokens[i + 1], out address, out port))
                {
                    return true;
                }
            }

            return TryParseToken(trimmed, out address, out port);
        }

        private static bool TryParseToken(string token, out string address, out int port)
        {
            address = string.Empty;
            port = 0;

            if (string.IsNullOrWhiteSpace(token))
            {
                return false;
            }

            var sanitized = StripConnectSyntax(token);
            if (string.IsNullOrWhiteSpace(sanitized))
            {
                return false;
            }

            sanitized = sanitized.Trim('"');

            if (Uri.TryCreate($"tcp://{sanitized}", UriKind.Absolute, out var uri) && !string.IsNullOrEmpty(uri.Host))
            {
                address = uri.Host;
                port = uri.IsDefaultPort ? NetworkDefaults.DefaultPort : uri.Port;
                return port > 0 && port <= 65535;
            }

            var parts = sanitized.Split(':');
            if (parts.Length == 2 && !string.IsNullOrWhiteSpace(parts[0]) && int.TryParse(parts[1], out port))
            {
                address = parts[0];
                return port > 0 && port <= 65535;
            }

            if (parts.Length == 1 && !string.IsNullOrWhiteSpace(parts[0]))
            {
                address = parts[0];
                port = NetworkDefaults.DefaultPort;
                return true;
            }

            return false;
        }

        private static string StripConnectSyntax(string token)
        {
            var sanitized = token.Trim('"').Trim();
            if (sanitized.Length == 0)
            {
                return sanitized;
            }

            if (sanitized.StartsWith("+connect", StringComparison.OrdinalIgnoreCase))
            {
                sanitized = sanitized.Substring("+connect".Length);
            }
            else if (sanitized.StartsWith("connect", StringComparison.OrdinalIgnoreCase))
            {
                sanitized = sanitized.Substring("connect".Length);
            }

            sanitized = sanitized.TrimStart('=', ':');

            var equalsIndex = sanitized.LastIndexOf('=');
            if (equalsIndex >= 0 && equalsIndex < sanitized.Length - 1)
            {
                sanitized = sanitized.Substring(equalsIndex + 1);
            }

            return sanitized.Trim();
        }

        private static bool IsConnectKeyword(string token)
        {
            var sanitized = token.Trim();
            return sanitized.Equals("+connect", StringComparison.OrdinalIgnoreCase) ||
                   sanitized.Equals("connect", StringComparison.OrdinalIgnoreCase);
        }
    }
}
