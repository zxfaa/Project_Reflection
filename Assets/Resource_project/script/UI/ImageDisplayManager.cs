using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ImageDisplayManager : MonoBehaviour
{
    public static ImageDisplayManager Instance;

    public Animator fadeAnimator; // 淡入淡出動畫
    public Image interactionImage; // 互動顯示的圖片
    public Text descriptionText; // 顯示圖片描述的文本
    public bool maintainAspectRatio = true; // 是否保持等比縮放

    private Queue<IEnumerator> imageQueue = new Queue<IEnumerator>();
    private bool isDisplaying = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void QueueImagesWithFade(ItemData itemData, int[] itemIndices)
    {
        imageQueue.Enqueue(ShowImagesWithFadeCoroutine(itemData, itemIndices));
        if (!isDisplaying)
        {
            StartCoroutine(ProcessQueue());
        }
    }

    private IEnumerator ProcessQueue()
    {
        isDisplaying = true;
        while (imageQueue.Count > 0)
        {
            yield return StartCoroutine(imageQueue.Dequeue());
        }
        isDisplaying = false;
    }

    private IEnumerator ShowImagesWithFadeCoroutine(ItemData itemData, int[] itemIndices)
    {
        for (int i = 0; i < itemIndices.Length; i++)
        {
            if (itemIndices[i] >= 0 && itemIndices[i] < itemData.items.Count)
            {
                var item = itemData.items[itemIndices[i]];
                interactionImage.sprite = item.itemIcon;
                descriptionText.text = item.itemName;
                interactionImage.preserveAspect = maintainAspectRatio; // 保持等比縮放
                ResizeToFit(interactionImage, item.itemIcon);

                yield return StartCoroutine(Fade(interactionImage, descriptionText));
            }
        }

        interactionImage.gameObject.SetActive(false);
        descriptionText.gameObject.SetActive(false);
    }

    private IEnumerator Fade(Image interactionImage, Text descriptionText)
    {
        interactionImage.gameObject.SetActive(true);
        descriptionText.gameObject.SetActive(true);

        fadeAnimator.SetTrigger("GetItemInTrigger");

        yield return new WaitForSeconds(3f);

        fadeAnimator.SetTrigger("GetItemOutTrigger");

        yield return new WaitForSeconds(1f);
    }

    // 符合UI大小
    private void ResizeToFit(Image image, Sprite sprite)
    {
        RectTransform imageRectTransform = image.rectTransform;
        float imageWidth = imageRectTransform.rect.width;
        float imageHeight = imageRectTransform.rect.height;

        float spriteWidth = sprite.bounds.size.x;
        float spriteHeight = sprite.bounds.size.y;

        float scaleFactor = Mathf.Min(imageWidth / spriteWidth, imageHeight / spriteHeight);

        imageRectTransform.sizeDelta = new Vector2(spriteWidth * scaleFactor, spriteHeight * scaleFactor);
    }
}
