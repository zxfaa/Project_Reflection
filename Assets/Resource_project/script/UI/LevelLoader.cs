using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen; // 使用 [SerializeField] 來確保可以在檢視器中設置
    [SerializeField] private Slider slider;

    void Awake()
    {
        if (loadingScreen == null || slider == null)
        {
            Debug.LogError("Loading screen or slider is not assigned in the inspector.");
            return;
        }

        loadingScreen.SetActive(false); // 確保在開始時隱藏加載畫面
    }

    public void LoadLevel(int sceneIndex)
    {
        StartCoroutine(LoadAsynchronously(sceneIndex));
    }

    private IEnumerator LoadAsynchronously(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        loadingScreen.SetActive(true);

        while (!operation.isDone)
        {
            slider.value = operation.progress; // 直接使用 operation.progress

            // Debug information
            Debug.Log($"Loading progress: {slider.value * 100}%");

            yield return null;
        }

        loadingScreen.SetActive(false); // 當場景加載完成後隱藏加載畫面
    }
}
