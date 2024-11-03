using UnityEngine;
using TMPro;

public class TheaterPuzzle : MonoBehaviour
{
    public TMP_InputField userInput;
    public string correctAnswer;

    private bool isCorrect;

    void Start()
    {
        userInput.onEndEdit.AddListener(OnInputFieldSubmit);
    }

    private void OnInputFieldSubmit(string input)
    {
        if (IsCorrect())
        {
            Finished();
            Debug.LogWarning("Correct");
        }
        else
        {
            Debug.LogWarning("Incorrect");
        }
    }

    private bool IsCorrect()
    {
        if (userInput.text == correctAnswer)
        {
            isCorrect = true;
        }
        else
        {
            isCorrect = false;
        }
        return isCorrect;
    }

    private void Finished()
    {

    }
}
