using UnityEngine;

public abstract class ItemEffectBase : ScriptableObject, IItemEffect
{
    public abstract void UseEffect(GameObject target);
}
