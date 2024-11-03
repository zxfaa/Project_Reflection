using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class GlowEffect : MonoBehaviour
{
    public Text subtitleText;
    public float fadeDuration = 0.25f;
    private Coroutine fadeCoroutine;
    public float maxAlpha = 1f;

    private Item itemGlow;

    void Start()
    {
        if (subtitleText != null)
        {
            SetAlpha(0f); // 設置初始透明度為 0
        }
        itemGlow = GetComponent<Item>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (gameObject.activeSelf)
        {
            if (other.CompareTag("Player") && CanInteract())
            {
                if (subtitleText != null)
                {
                    if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
                    fadeCoroutine = StartCoroutine(ChangeTextAlpha(0f, maxAlpha));
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (gameObject.activeSelf)
        {
            if (other.CompareTag("Player") && CanInteract())
            {
                if (subtitleText != null)
                {
                    if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
                    fadeCoroutine = StartCoroutine(ChangeTextAlpha(maxAlpha, 0f));
                }
            }
        }
    }

    IEnumerator ChangeTextAlpha(float from, float to)
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            float alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            SetAlpha(alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }
        SetAlpha(to);
    }

    void SetAlpha(float alpha)
    {
        if (subtitleText != null)
        {
            Color color = subtitleText.color;
            color.a = alpha;
            subtitleText.color = color;
        }
    }

    bool CanInteract()
    {
        return itemGlow == null || InteractionManager.Instance.CanInteract(itemGlow.objectId);
    }
}
