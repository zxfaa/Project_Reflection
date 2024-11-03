using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class TriggerSubtitle : MonoBehaviour
{
    public FadeController fadeController; // 連接到 FadeController
    public SubtitleData subtitleData;     // 連接到 SubtitleData 資源 

    private void Reset()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //根據觸發物件的名稱
            var subtitleGroup = subtitleData.GetSubtitleGroupByName(gameObject.name);
            if (subtitleGroup != null && !subtitleGroup.hasBeenTriggered)
            {
                //僅當字幕組未曾被觸發時，才顯示字幕
                fadeController.DisplaySubtitles(subtitleGroup);

                //標記為以觸發
                subtitleGroup.hasBeenTriggered = true;

                //觸發後銷毀
                Destroy(gameObject);
            }
        }
    }
}
