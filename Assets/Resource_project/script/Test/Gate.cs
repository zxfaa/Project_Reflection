using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Gate : MonoBehaviour
{
    public TMP_InputField userInput;
    public TMP_Text text;

    private string gatePassword = "9586478";
    private bool isCorrect;

    public Image fadeImage;
    public Animator fadeAnimator;
    LevelLoader levelLoader;

    private void Start()
    {

        levelLoader = FindObjectOfType<LevelLoader>();
        if (levelLoader == null)
        {
            Debug.LogError("LevelLoader not found in the scene!");
        }
        userInput.onEndEdit.AddListener(OnInputFieldSubmit);
        text.enabled = false;
    }

    public void EnterPassword()
    {
        if (IsCorrect())
        {
            Finished();
        }
        else
        {
            Debug.LogWarning("Incorrect");
        }
    }

    private bool IsCorrect()
    {
        if (userInput.text == gatePassword)
        {
            isCorrect = true;
            text.text = "Correct";
            text.color = Color.green;
            text.enabled = true;
        }
        else
        {
            isCorrect = false;
            text.text = "Incorrect";
            text.color = Color.red;
            text.enabled = true;
        }

        return isCorrect;
    }

    private void OnInputFieldSubmit(string input)
    {
        if (IsCorrect())
        {
            Finished();
        }
        else
        {
            Debug.LogWarning("Incorrect");
        }
    }

    private void Finished()
    {

        Debug.Log("Finished");
        levelLoader.LoadLevel(3);
       

    }
}
