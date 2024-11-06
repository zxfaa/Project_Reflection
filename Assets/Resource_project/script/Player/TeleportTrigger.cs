using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportTrigger : MonoBehaviour
{
    public int teleportIndex;
    public int locationId;


    public void Teleport()
    {
        if (!FindObjectOfType<TeleportManager>().isTeleport)
        {
            TeleportManager manager = FindObjectOfType<TeleportManager>();
            if (manager != null)
            {
                // 觸發傳送
                manager.TriggerTeleport(teleportIndex, locationId);
                AudioManager.Instance.PlayOneShot("OpenDoor");
            }
        }
    }


}
