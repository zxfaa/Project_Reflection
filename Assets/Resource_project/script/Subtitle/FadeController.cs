using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class FadeController : MonoBehaviour
{
    public CanvasGroup canvasGroup;   // 用於淡入淡出字幕的 CanvasGroup
    public float fadeDuration = 1.0f; // 淡入淡出所需的時間
    private Coroutine currentCoroutine = null; // 當前正在運行的協程

    // 顯示字幕組
    public void DisplaySubtitles(SubtitleData.SubtitleGroup subtitleGroup)
    {
        // 如果有協程正在運行，停止它
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }

        // 開始顯示新的字幕協程
        currentCoroutine = StartCoroutine(DisplaySubtitleCoroutine(subtitleGroup.subtitles));
    }

    // 協程：顯示一個字幕組中的所有字幕
    private IEnumerator DisplaySubtitleCoroutine(List<SubtitleData.Subtitle> subtitles)
    {
        Text textComponent = canvasGroup.GetComponentInChildren<Text>();

        foreach (var subtitle in subtitles)
        {
            textComponent.text = subtitle.text;

            // 淡入字幕
            yield return StartCoroutine(FadeCanvasGroup(canvasGroup, 0, 1, fadeDuration));

            // 顯示字幕一段時間
            yield return new WaitForSeconds(subtitle.displayDuration);

            // 淡出字幕
            yield return StartCoroutine(FadeCanvasGroup(canvasGroup, 1, 0, fadeDuration));
        }

        // 當顯示完畢後，重置當前協程引用
        currentCoroutine = null;
    }

    // 協程：淡入或淡出 CanvasGroup
    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float duration)
    {
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            cg.alpha = Mathf.Lerp(start, end, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cg.alpha = end;
    }
}
