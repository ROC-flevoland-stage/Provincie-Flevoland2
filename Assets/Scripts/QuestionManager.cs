using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestionManager : MonoBehaviour
{
    public TextMeshProUGUI questionTextUI;



    public List<Question> questions = new List<Question>();

    private int currentIndex = 0;

    public Question CurrentQuestion => questions[currentIndex];
    
    void Start()
    { // Voorbeeldvragen

        questions.Add(new Question { questionText = "Hoe is je lichamelijke gezondheid?", answer = 0 });
        questions.Add(new Question { questionText = "Hoe is je mentale Gezondheid?", answer = 0 });
        questions.Add(new Question { questionText = "Hoe is je hoeveelheid vrije tijd?", answer = 0 });
        questions.Add(new Question { questionText = "En de veiligheid in je buurt?", answer = 0 });
        questions.Add(new Question { questionText = "En hoe zou jij de natuur in je leefomgeving geven?", answer = 0 });

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
            Debug.Log("All questions completed!");
            return;
        }

        DisplayQuestion();
    }
    public void DisplayQuestion()
    {
        questionTextUI.text = CurrentQuestion.questionText;
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