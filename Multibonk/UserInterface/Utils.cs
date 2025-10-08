using UnityEngine;

public static class Utils
{
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


        if (e.type == EventType.MouseDown)
        {
            isFocused = rect.Contains(e.mousePosition);

        }


        if (isFocused && e.type == EventType.KeyDown)
        {
            if (e.keyCode == KeyCode.Backspace && text.Length > 0)
                text = text.Substring(0, text.Length - 1);
            else if (e.character != '\0' && !char.IsControl(e.character))
                text += e.character;

            e.Use();
        }
    }

    public static string CustomTextField(string currentText, ref bool isFocused, Rect rect)
    {
        // not working
        Rect calculated = GUILayoutUtility.GetRect(new GUIContent(currentText), GUI.skin.textField, GUILayout.Width(rect.width), GUILayout.Height(rect.height));

        HandleTextFieldInput(ref currentText, ref isFocused, calculated);
        GUI.Box(calculated, currentText);
        return currentText;
    }
}