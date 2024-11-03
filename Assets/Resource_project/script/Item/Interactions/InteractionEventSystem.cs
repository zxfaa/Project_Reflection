using System;
using UnityEngine;

public static class InteractionEventSystem
{
    public static event Action<bool> OnLockStateChanged;

    public static void RaiseLockStateChanged(bool isLocked)
    {
        OnLockStateChanged?.Invoke(isLocked);
    }
}
