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
        int controlId = GUIUtility.GetControlID(FocusType.Keyboard);
        Rect calculated = GUILayoutUtility.GetRect(new GUIContent(currentText), style, options.ToArray());

        currentText = HandleTextFieldInput(currentText, calculated, controlId, ref isFocused);

        if (Event.current.type == EventType.Repaint)
        {
            style.Draw(calculated, new GUIContent(currentText), controlId);
        }

        return currentText;
    }

    private static string HandleTextFieldInput(string text, Rect rect, int controlId, ref bool isFocused)
    {
        Event e = Event.current;

        switch (e.type)
        {
            case EventType.MouseDown:
                if (GUI.enabled && rect.Contains(e.mousePosition))
                {
                    GUIUtility.hotControl = controlId;
                    GUIUtility.keyboardControl = controlId;
                    isFocused = true;
                    e.Use();
                }
                else if (GUIUtility.keyboardControl == controlId)
                {
                    GUIUtility.keyboardControl = 0;
                    isFocused = false;
                }
                break;

            case EventType.MouseDrag:
                if (GUIUtility.hotControl == controlId)
                {
                    e.Use();
                }
                break;

            case EventType.MouseUp:
                if (GUIUtility.hotControl == controlId)
                {
                    GUIUtility.hotControl = 0;
                    e.Use();
                }
                break;

            case EventType.KeyDown:
                if (GUIUtility.keyboardControl == controlId && GUI.enabled)
                {
                    switch (e.keyCode)
                    {
                        case KeyCode.Backspace:
                        case KeyCode.Delete:
                            if (text.Length > 0)
                            {
                                text = text.Substring(0, text.Length - 1);
                                GUI.changed = true;
                            }
                            e.Use();
                            break;

                        case KeyCode.Return:
                        case KeyCode.KeypadEnter:
                        case KeyCode.Escape:
                        case KeyCode.Tab:
                            GUIUtility.keyboardControl = 0;
                            isFocused = false;
                            e.Use();
                            break;

                        default:
                            char c = e.character;
                            if (!char.IsControl(c))
                            {
                                text += c;
                                GUI.changed = true;
                                e.Use();
                            }
                            break;
                    }
                }
                break;
        }

        if (!GUI.enabled && GUIUtility.keyboardControl == controlId)
        {
            GUIUtility.keyboardControl = 0;
            isFocused = false;
        }

        if (!GUI.enabled && GUIUtility.hotControl == controlId)
        {
            GUIUtility.hotControl = 0;
        }

        if (GUIUtility.keyboardControl != controlId && isFocused)
        {
            isFocused = false;
        }
        else if (GUIUtility.keyboardControl == controlId && !isFocused)
        {
            isFocused = true;
        }

        return text;
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
