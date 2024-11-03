using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class FrontDoor : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Transform player;
    public float fadeSpeed = 2f;

    Color color;
    float targetAlpha = 0f;

    // Start is called before the first frame update
    void Start()
    {
        color = spriteRenderer.color;
        color.a = 0;
        spriteRenderer.color = color;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            targetAlpha = 1f;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            targetAlpha = 0f;
    }

    private void Update()
    {
        color.a = Mathf.MoveTowards(color.a, targetAlpha, fadeSpeed * Time.deltaTime);
        spriteRenderer.color = color;
    }
}
