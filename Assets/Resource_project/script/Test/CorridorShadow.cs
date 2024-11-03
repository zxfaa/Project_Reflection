using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorridorShadow : MonoBehaviour
{
    public Animator animatorShadow;
    public GameObject shadow;
    public Transform target;
    public float fadeDistance = 3f;
    public float speed = 10f;
    public static bool ShadowTrigger = false;
    
    private bool isMoving;
    private SpriteRenderer[] spriteRenderer;

    private void Start()
    {
        spriteRenderer = shadow.GetComponentsInChildren<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !ShadowTrigger)
        {
            animatorShadow.SetBool("isMoving", true);
            isMoving = true;
            ShadowTrigger = true;
        }
    }

    private void Update()
    {
        if (isMoving)
        {
            shadow.transform.position = Vector2.MoveTowards(shadow.transform.position, target.position, speed * Time.deltaTime);
            float distance = Vector2.Distance(shadow.transform.position, target.position);
            if (distance < fadeDistance)
            {
                float alpha = Mathf.Lerp(1f, 0f, 1 - (distance / fadeDistance));  // 計算 alpha 值
                SetAlpha(alpha);  // 設置透明度
            }

            if (distance < 0.1f)
            {
                isMoving = false;
                animatorShadow.SetBool("isMoving", false);  // 停止動畫
                Destroy(gameObject);
                Destroy(shadow);
            }
        }
    }

    private void SetAlpha(float alpha)
    {
        for (int i = 0 ; i < spriteRenderer.Length; i++)
        {
            Color color = spriteRenderer[i].color;
            color.a = alpha;
            spriteRenderer[i].color = color;
        }
        
    }
}
