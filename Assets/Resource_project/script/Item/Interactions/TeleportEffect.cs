using UnityEngine;

[CreateAssetMenu(fileName = "TeleportEffect", menuName = "ItemEffects/TeleportEffect")]
public class TeleportEffect : ItemEffectBase
{
    public int teleportIndex;

    public override void UseEffect(GameObject target)
    {
        /*Debug.Log("TeleportEffect 被執行");
        TeleportManager manager = FindObjectOfType<TeleportManager>();
        if (manager != null)
        {
            manager.UseKey();
            if (manager.isKeyUsed)
            {
                Debug.Log("鑰匙已使用");
                manager.UnlockDoor();
            }
            else
            {
                Debug.Log("請先使用鑰匙解鎖門");
            }
        }
        else
        {
            Debug.LogError("TeleportManager not found.");
        }*/
    }
}
