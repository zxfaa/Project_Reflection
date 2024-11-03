using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelCrossFade : MonoBehaviour
{
    public Image fadeImage;

    private void Start()
    {
        fadeImage.gameObject.SetActive(true);
    }
    // Start is called before the first frame update
}
