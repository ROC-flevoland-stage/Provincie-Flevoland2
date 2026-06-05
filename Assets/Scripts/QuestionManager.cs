using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuestionManager : MonoBehaviour
{
    public TextMeshProUGUI questionTextUI;


    [SerializeField]
    public List<Question> questions = new List<Question>();

    private int currentIndex = 0;

    public Question CurrentQuestion => questions[currentIndex];

    
    void Start()
    { // Voorbeeldvragen

        DisplayQuestion();
    }

    // Speler's antwoord van 1-10
    public void SubmitAnswer(int value)
    {
        CurrentQuestion.answer = value;

        Debug.Log($"Q{currentIndex + 1} answered: {value}");

        NextQuestion();
    }

    public void NextQuestion()
    {
        currentIndex++;

        if (currentIndex >= questions.Count)
        {
            endMinigame();
        }

        DisplayQuestion();
    }
    public void DisplayQuestion()
    {
        questionTextUI.text = CurrentQuestion.questionText;
    }

    private void endMinigame()
    {
        Debug.Log("All questions completed!");

        foreach (Question question in questions)
        {
            SaveManager.CreateOrSetValue(question.questionID, question.answer, true);
        }

        //Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("E3Demo");

        return;
    }

    // Voor snelle toetsen 1-10 om antwoorden in te voeren zonder de mini-game. (Testen)
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SubmitAnswer(1);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SubmitAnswer(2);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SubmitAnswer(3);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SubmitAnswer(4);
        if (Input.GetKeyDown(KeyCode.Alpha5)) SubmitAnswer(5);
        if (Input.GetKeyDown(KeyCode.Alpha6)) SubmitAnswer(6);
        if (Input.GetKeyDown(KeyCode.Alpha7)) SubmitAnswer(7);
        if (Input.GetKeyDown(KeyCode.Alpha8)) SubmitAnswer(8);
        if (Input.GetKeyDown(KeyCode.Alpha9)) SubmitAnswer(9);
        if (Input.GetKeyDown(KeyCode.Alpha0)) SubmitAnswer(10);
    }
}