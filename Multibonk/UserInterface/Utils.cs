using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    private static GUISkin toolbarCachedSkin;
    private static GUIStyle toolbarButtonStyle;
    private static GUIStyle toolbarButtonSelectedStyle;

    public static void HandleWindowDrag(ref Rect window, ref bool dragging, ref Vector2 dragOffset)
    {
        Event e = Event.current;
        Rect dragBar = new Rect(window.x, window.y, window.width, 20);

        if (e.type == EventType.MouseDown && dragBar.Contains(e.mousePosition))
        {
            dragging = true;
            dragOffset = e.mousePosition - new Vector2(window.x, window.y);
            e.Use();
        }
        else if (e.type == EventType.MouseUp)
        {
            dragging = false;
        }

        if (dragging && e.type == EventType.MouseDrag)
        {
            window.position = e.mousePosition - dragOffset;
            e.Use();
        }
    }

    public static void HandleTextFieldInput(ref string text, ref bool isFocused, Rect rect)
    {
        Event e = Event.current;

        if (!GUI.enabled)
        {
            isFocused = false;
            return;
        }

        if (e.type == EventType.MouseDown)
        {
            isFocused = rect.Contains(e.mousePosition);
        }

        if (isFocused && e.type == EventType.KeyDown)
        {
            if ((e.keyCode == KeyCode.Backspace || e.keyCode == KeyCode.Delete) && text.Length > 0)
            {
                text = text.Substring(0, text.Length - 1);
            }
            else if (e.keyCode == KeyCode.Return || e.keyCode == KeyCode.KeypadEnter || e.keyCode == KeyCode.Escape || e.keyCode == KeyCode.Tab)
            {
                isFocused = false;
            }
            else if (e.character != '\0' && !char.IsControl(e.character))
            {
                text += e.character;
            }

            e.Use();
        }
    }

    public static string CustomTextField(string currentText, ref bool isFocused, Rect rect)
    {
        currentText ??= string.Empty;

        var options = new List<GUILayoutOption>();
        if (rect.width > 0f)
        {
            options.Add(GUILayout.Width(rect.width));
        }
        else
        {
            options.Add(GUILayout.ExpandWidth(true));
        }

        if (rect.height > 0f)
        {
            options.Add(GUILayout.Height(rect.height));
        }

        GUIStyle style = GUI.skin.textField;
        Rect calculated = GUILayoutUtility.GetRect(new GUIContent(currentText), style, options.ToArray());

        HandleTextFieldInput(ref currentText, ref isFocused, calculated);

        GUI.Box(calculated, currentText, style);
        return currentText;
    }

    public static int CustomToolbar(int selectedIndex, string[] labels, params GUILayoutOption[] options)
    {
        if (labels == null || labels.Length == 0)
        {
            return selectedIndex;
        }

        EnsureToolbarStyles();

        int newIndex = selectedIndex;
        GUILayout.BeginHorizontal(options);
        for (int i = 0; i < labels.Length; i++)
        {
            bool isSelected = i == selectedIndex;
            GUIStyle style = isSelected ? toolbarButtonSelectedStyle : toolbarButtonStyle;
            bool pressed = GUILayout.Toggle(isSelected, labels[i] ?? string.Empty, style);
            if (pressed && !isSelected)
            {
                newIndex = i;
            }
        }
        GUILayout.EndHorizontal();

        return newIndex;
    }

    private static void EnsureToolbarStyles()
    {
        if (toolbarCachedSkin == GUI.skin && toolbarButtonStyle != null && toolbarButtonSelectedStyle != null)
        {
            return;
        }

        toolbarCachedSkin = GUI.skin;
        toolbarButtonStyle = new GUIStyle(GUI.skin.button);
        toolbarButtonSelectedStyle = new GUIStyle(GUI.skin.button);

        CopyState(toolbarButtonSelectedStyle.active, toolbarButtonSelectedStyle.normal);
        CopyState(toolbarButtonSelectedStyle.active, toolbarButtonSelectedStyle.hover);
        CopyState(toolbarButtonSelectedStyle.active, toolbarButtonSelectedStyle.focused);
        CopyState(toolbarButtonSelectedStyle.active, toolbarButtonSelectedStyle.onNormal);
        CopyState(toolbarButtonSelectedStyle.active, toolbarButtonSelectedStyle.onHover);
        CopyState(toolbarButtonSelectedStyle.active, toolbarButtonSelectedStyle.onFocused);
        CopyState(toolbarButtonSelectedStyle.active, toolbarButtonSelectedStyle.onActive);

        if (toolbarButtonSelectedStyle.normal.background == null && toolbarButtonStyle.active.background != null)
        {
            toolbarButtonSelectedStyle.normal.background = toolbarButtonStyle.active.background;
        }
        if (toolbarButtonSelectedStyle.normal.textColor == default)
        {
            toolbarButtonSelectedStyle.normal.textColor = toolbarButtonStyle.active.textColor;
        }
    }

    private static void CopyState(GUIStyleState source, GUIStyleState destination)
    {
        if (source == null || destination == null)
        {
            return;
        }

        destination.background = source.background;
        destination.textColor = source.textColor;
    }
}
