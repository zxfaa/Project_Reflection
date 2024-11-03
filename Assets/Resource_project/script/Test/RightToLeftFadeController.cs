using UnityEngine;

public class LeftToRightFadeController : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public float fadeSpeed = 0.5f; // 控制淡出速度
    public bool fade = false;

    private Material material;
    private float fadeValue = 0.7f; // 初始值，確保左邊不透明、右邊透明

    void Start()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (spriteRenderer != null)
        {
            material = spriteRenderer.material;
            if (material != null)
            {
                material.SetFloat("_Fade", fadeValue);
            }
            else
            {
                Debug.LogError("SpriteRenderer does not have a valid material with the LeftToRightFadeOut shader.");
            }
        }
    }

    void Update()
    {
        if (fade)
            FadeShadow();
    }

    void FadeShadow()
    {
        if (material != null)
        {
            // 逐漸增加 fadeValue，讓透明度從左往右淡出，最終完全透明
            fadeValue -= fadeSpeed * Time.deltaTime;
            material.SetFloat("_Fade", fadeValue);
            if (fadeValue <= -0.5)
                gameObject.SetActive(false);
        }
    }
}
