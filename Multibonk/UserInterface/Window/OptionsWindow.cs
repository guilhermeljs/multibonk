using System;
using System.Globalization;
using UnityEngine;

namespace Multibonk.UserInterface.Window
{
    public class OptionsWindow : WindowBase
    {
        private bool isOpen;
        private bool pvpEnabled;
        private bool reviveEnabled;
        private string reviveDelayInput = string.Empty;
        private float reviveDelaySeconds;
        private string reviveDelayError = string.Empty;
        private bool reviveDelayFieldFocused;
        private Preferences.LootDistributionMode xpMode;
        private Preferences.LootDistributionMode goldMode;
        private Preferences.LootDistributionMode chestMode;
        private bool steamOverlayAvailable;
        private string steamTunnelStatus = string.Empty;

        private GUISkin cachedSkin;
        private GUIStyle windowLabelStyle;
        private GUIStyle sectionTitleStyle;
        private GUIStyle descriptionLabelStyle;
        private GUIStyle toggleStyle;
        private GUIStyle errorLabelStyle;

        public event Action PreferencesChanged;
        public event Action OpenSteamOverlayRequested;

        private const float WindowWidth = 520f;
        private const float WindowHeight = 520f;

        public OptionsWindow() : base(new Rect(80f, 80f, WindowWidth, WindowHeight))
        {
            RefreshFromPreferences();
        }

        public bool IsOpen => isOpen;

        public void Show()
        {
            RefreshFromPreferences();
            isOpen = true;
        }

        public void Hide()
        {
            isOpen = false;
        }

        public void SetSteamOverlayAvailability(bool available)
        {
            steamOverlayAvailable = available;
        }

        public void SetSteamTunnelStatus(string status)
        {
            steamTunnelStatus = status;
        }

        protected override void RenderWindow(Rect rect)
        {
            if (!isOpen)
            {
                return;
            }

            GUILayout.BeginArea(rect, GUI.skin.window);
            GUI.Box(new Rect(0, 0, rect.width, rect.height), GUIContent.none, GUI.skin.window);

            EnsureStyles();

            var labelStyle = windowLabelStyle;

            GUILayout.Label("Gameplay Options", labelStyle);

            DrawToggleSection("Player Versus Player", "Allow players to damage each other.", ref pvpEnabled, value =>
            {
                Preferences.PvpEnabled.Value = value;
            });

            DrawToggleSection("Revive Mechanics", "Allow teammates to revive each other when they go down.", ref reviveEnabled, value =>
            {
                Preferences.ReviveEnabled.Value = value;
            });

            GUILayout.BeginHorizontal();
            GUILayout.Label("Revive delay (seconds):", labelStyle, GUILayout.Width(180));
            bool previousGuiState = GUI.enabled;
            GUI.enabled = reviveEnabled;
            string newInput = Utils.CustomTextField(reviveDelayInput, ref reviveDelayFieldFocused, new Rect(0f, 0f, 80f, 22f));
            GUI.enabled = previousGuiState;

            if (newInput != reviveDelayInput)
            {
                reviveDelayInput = newInput;
                if (float.TryParse(newInput, NumberStyles.Float, CultureInfo.InvariantCulture, out var seconds) && seconds > 0f)
                {
                    reviveDelaySeconds = seconds;
                    Preferences.ReviveTimeSeconds.Value = seconds;
                    PreferencesChanged?.Invoke();
                    reviveDelayError = string.Empty;
                }
                else if (reviveEnabled)
                {
                    reviveDelayError = "Enter a positive number (example: 5 or 7.5).";
                }
                else
                {
                    reviveDelayError = string.Empty;
                }
            }

            if (!string.IsNullOrEmpty(reviveDelayError) && reviveEnabled)
            {
                GUILayout.Label(reviveDelayError, errorLabelStyle);
            }

            GUILayout.EndHorizontal();
            AddVerticalSpace(10f);

            DrawDistributionSection("Experience sharing", ref xpMode, Preferences.SetXpSharingMode,
                "Shared: everyone gains XP together.",
                "Individual: only the collector gains XP.",
                "Duplicated: each drop spawns for every player.");

            AddVerticalSpace(6f);

            DrawDistributionSection("Gold sharing", ref goldMode, Preferences.SetGoldSharingMode,
                "Shared: the team uses one shared wallet.",
                "Individual: everyone keeps their own gold.",
                "Duplicated: pickups reward every player equally.");

            AddVerticalSpace(6f);

            DrawDistributionSection("Chest rewards", ref chestMode, Preferences.SetChestSharingMode,
                "Shared: every chest rewards the whole team.",
                "Individual: each player must open the chest themselves.",
                "Duplicated: each chest can be opened once per player.");

            AddVerticalSpace(12f);

            DrawSteamOverlaySection();

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Close"))
            {
                Hide();
            }

            GUILayout.EndArea();
        }

        private void DrawToggleSection(string title, string description, ref bool cache, Action<bool> setter)
        {
            bool newValue = GUILayout.Toggle(cache, title, toggleStyle);
            if (newValue != cache)
            {
                cache = newValue;
                setter(cache);
                PreferencesChanged?.Invoke();
            }

            GUILayout.Label(description, descriptionLabelStyle);
        }

        private void DrawDistributionSection(string title,
            ref Preferences.LootDistributionMode cache,
            Action<Preferences.LootDistributionMode> setter,
            string sharedDescription,
            string individualDescription,
            string duplicatedDescription)
        {
            GUILayout.Label(title, sectionTitleStyle);

            string[] labels = { "Shared", "Individual", "Duplicated" };
            int currentIndex = (int)cache;
            int selectedIndex = Utils.CustomToolbar(currentIndex, labels);
            if (selectedIndex != currentIndex)
            {
                cache = (Preferences.LootDistributionMode)selectedIndex;
                setter(cache);
                PreferencesChanged?.Invoke();
            }

            string description = cache switch
            {
                Preferences.LootDistributionMode.Shared => sharedDescription,
                Preferences.LootDistributionMode.Individual => individualDescription,
                _ => duplicatedDescription
            };

            GUILayout.Label(description, descriptionLabelStyle);
        }

        private void RefreshFromPreferences()
        {
            pvpEnabled = Preferences.PvpEnabled.Value;
            reviveEnabled = Preferences.ReviveEnabled.Value;
            reviveDelaySeconds = Preferences.ReviveTimeSeconds.Value;
            reviveDelayInput = reviveDelaySeconds.ToString("0.##", CultureInfo.InvariantCulture);
            reviveDelayFieldFocused = false;
            xpMode = Preferences.GetXpSharingMode();
            goldMode = Preferences.GetGoldSharingMode();
            chestMode = Preferences.GetChestSharingMode();
            reviveDelayError = string.Empty;
        }

        private void DrawSteamOverlaySection()
        {
            GUILayout.Label("Steam tunneling", sectionTitleStyle);
            GUILayout.Label("Use the Steam overlay to discover and join friend lobbies.", descriptionLabelStyle);

            if (!string.IsNullOrEmpty(steamTunnelStatus))
            {
                GUILayout.Label(steamTunnelStatus, descriptionLabelStyle);
            }

            bool previous = GUI.enabled;
            GUI.enabled = steamOverlayAvailable;
            if (GUILayout.Button("Open Steam Friends Overlay"))
            {
                OpenSteamOverlayRequested?.Invoke();
            }
            GUI.enabled = previous;
        }

        private void EnsureStyles()
        {
            if (cachedSkin == GUI.skin && windowLabelStyle != null && sectionTitleStyle != null &&
                descriptionLabelStyle != null && toggleStyle != null && errorLabelStyle != null)
            {
                return;
            }

            cachedSkin = GUI.skin;

            windowLabelStyle = new GUIStyle(GUI.skin.label)
            {
                wordWrap = true
            };
            windowLabelStyle.normal.textColor = Color.white;

            sectionTitleStyle = new GUIStyle(GUI.skin.label)
            {
                fontStyle = FontStyle.Bold
            };
            sectionTitleStyle.normal.textColor = Color.white;

            descriptionLabelStyle = new GUIStyle(windowLabelStyle);

            toggleStyle = new GUIStyle(GUI.skin.toggle);
            ApplyWhiteText(toggleStyle);

            errorLabelStyle = new GUIStyle(windowLabelStyle);
            errorLabelStyle.normal.textColor = Color.red;
        }

        private static void AddVerticalSpace(float pixels)
        {
            GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(pixels));
        }

        private static void ApplyWhiteText(GUIStyle style)
        {
            style.normal.textColor = Color.white;
            style.onNormal.textColor = Color.white;
            style.hover.textColor = Color.white;
            style.onHover.textColor = Color.white;
            style.active.textColor = Color.white;
            style.onActive.textColor = Color.white;
            style.focused.textColor = Color.white;
            style.onFocused.textColor = Color.white;
        }
    }
}
