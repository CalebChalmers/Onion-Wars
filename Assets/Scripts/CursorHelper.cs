using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CursorHelper
{
    private static bool _cursorLocked = false;

    public static bool CursorLocked {
        get
        {
            return _cursorLocked;
        }
        set
        {
            if(_cursorLocked != value)
            {
                Cursor.visible = !value;
                Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.None;
                _cursorLocked = value;
            }
        }
    }
}
