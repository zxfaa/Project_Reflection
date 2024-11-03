using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NarcissusController : MonoBehaviour
{
    public GameObject narcissus;

    void Start()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        if (currentSceneName == "stage2" ||  currentSceneName == "stage3")
            narcissus.SetActive(true);
        else
            narcissus.SetActive(false);
    }
}
