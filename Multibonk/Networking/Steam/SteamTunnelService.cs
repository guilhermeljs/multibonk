using System;
using System.Collections.Concurrent;
using System.Reflection;
using MelonLoader;

namespace Multibonk.Networking.Steam
{
    public class SteamTunnelService
    {
        private readonly ConcurrentQueue<SteamTunnelEndpoint> pendingEndpoints = new();
        private readonly object syncRoot = new();

        private MethodInfo? activateOverlayMethod;
        private Type? steamFriendsType;
        private bool missingSteamFriendsLogged;
        private bool missingOverlayMethodLogged;

        public SteamTunnelService()
        {
            AppDomain.CurrentDomain.AssemblyLoad += HandleAssemblyLoaded;
            ResolveSteamOverlay();
        }

        public bool IsOverlayAvailable => activateOverlayMethod != null;

        public bool HasPendingEndpoint => pendingEndpoints.TryPeek(out _);

        internal Type? SteamFriendsType => steamFriendsType;

        public void RegisterEndpoint(string address, int port)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                MelonLogger.Warning("Ignoring Steam tunnel endpoint with an empty address.");
                return;
            }

            if (port <= 0 || port > 65535)
            {
                MelonLogger.Warning($"Ignoring Steam tunnel endpoint with invalid port: {port}.");
                return;
            }

            pendingEndpoints.Enqueue(new SteamTunnelEndpoint(address, port));
            MelonLogger.Msg($"Registered Steam tunnel endpoint {address}:{port}.");
        }

        public bool TryPeekEndpoint(out SteamTunnelEndpoint endpoint) => pendingEndpoints.TryPeek(out endpoint);

        public bool TryConsumeEndpoint(out SteamTunnelEndpoint endpoint) => pendingEndpoints.TryDequeue(out endpoint);

        public void ClearEndpoints()
        {
            while (pendingEndpoints.TryDequeue(out _))
            {
            }
        }

        public bool TryOpenFriendsOverlay()
        {
            if (activateOverlayMethod == null)
            {
                MelonLogger.Warning("Steam overlay is unavailable. Ensure Steam is running before enabling Steam tunneling.");
                return false;
            }

            try
            {
                activateOverlayMethod.Invoke(null, new object[] { "Friends" });
                MelonLogger.Msg("Opened Steam friends overlay for tunneling.");
                return true;
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"Failed to open Steam overlay: {ex.Message}");
                return false;
            }
        }

        private void HandleAssemblyLoaded(object? sender, AssemblyLoadEventArgs args)
        {
            ResolveSteamOverlay();
        }

        private void ResolveSteamOverlay()
        {
            lock (syncRoot)
            {
                if (activateOverlayMethod != null)
                {
                    return;
                }

                var locatedSteamFriends = SteamFriendsReflection.LocateSteamFriendsType();
                if (locatedSteamFriends == null)
                {
                    if (!missingSteamFriendsLogged)
                    {
                        MelonLogger.Warning("Could not locate Steamworks.SteamFriends. Steam tunneling will remain disabled until Steamworks initialises.");
                        missingSteamFriendsLogged = true;
                    }

                    return;
                }

                steamFriendsType = locatedSteamFriends;

                activateOverlayMethod = SteamFriendsReflection.FindActivateGameOverlay(locatedSteamFriends);
                if (activateOverlayMethod != null)
                {
                    MelonLogger.Msg($"Steam overlay integration ready using '{locatedSteamFriends.Assembly.FullName}'.");
                    missingOverlayMethodLogged = false;
                    return;
                }

                if (!missingOverlayMethodLogged)
                {
                    MelonLogger.Warning("Could not locate SteamFriends.ActivateGameOverlay. Steam tunneling will remain disabled until the overlay becomes available.");
                    missingOverlayMethodLogged = true;
                }
            }
        }

    }

    public readonly struct SteamTunnelEndpoint : IEquatable<SteamTunnelEndpoint>
    {
        public string Address { get; }
        public int Port { get; }

        public SteamTunnelEndpoint(string address, int port)
        {
            Address = address;
            Port = port;
        }

        public override string ToString() => $"{Address}:{Port}";

        public bool Equals(SteamTunnelEndpoint other) =>
            string.Equals(Address, other.Address, StringComparison.OrdinalIgnoreCase) && Port == other.Port;

        public override bool Equals(object? obj) => obj is SteamTunnelEndpoint other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Address.ToLowerInvariant(), Port);
    }
}
