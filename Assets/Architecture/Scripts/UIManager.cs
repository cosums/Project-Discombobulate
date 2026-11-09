using System;
using UnityEngine;

public static class UIManager
{
    public static bool displayingUI;
    private static bool cursorEnabled;
    public static Action<bool> OnCursorStateChanged;

    public static void EnableCursor()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        cursorEnabled = true;
        OnCursorStateChanged?.Invoke(cursorEnabled);
    }

    public static void DisableCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cursorEnabled = false;
        OnCursorStateChanged?.Invoke(cursorEnabled);
    }
}
