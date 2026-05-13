using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class QuestionsManager : MonoBehaviour
{
    public TMP_Text questionText;      // assign hier de text UI
    public TMP_Text progressText;      // 1-10
    public string[] questions;         // hier 11 questions assignen

    int currentIndex;
    int[] answers;                    // stores je keuzes (1-10) per vraag
    bool finished;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (questions == null || questions.Length == 0)
        {
            Debug.LogWarning("QuestionsManager: no questions assigned.");
            finished = true;
            return;
        }

        answers = new int[questions.Length];
        currentIndex = 0;
        finished = false;
        ShowQuestion();
    }

    // New method name requested by you
    public void ChoiceSelected(int choice)
    {
        if (finished) return;

        // store keuze (1-10)
        answers[currentIndex] = choice;
        Debug.Log($"Question {currentIndex + 1} answer: {choice}");

        currentIndex++;
        if (currentIndex >= questions.Length)
            EndQuiz();
        else
            ShowQuestion();
    }

    // Backwards-compatible wrapper so existing callers using SelectChoice still work
    public void SelectChoice(int choice) => ChoiceSelected(choice);

    void ShowQuestion()
    {
        if (questionText != null)   
            questionText.text = questions[currentIndex];
        if (progressText != null)
            progressText.text = $"Question {currentIndex + 1}/{questions.Length}";
    }

    void EndQuiz()
    {
        finished = true;
        if (questionText != null)
            questionText.text = "Finished";
        if (progressText != null)
            progressText.text = $"Finished";

        Debug.Log("Quiz finished. Answers: " + string.Join(", ", System.Array.ConvertAll(answers, a => a.ToString())));
        SceneManager.LoadScene("E3Demo");
    }
}
