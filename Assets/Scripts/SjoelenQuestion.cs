using TMPro;
using UnityEngine;

public class SjoelenQuestion : MonoBehaviour
{
    public int QuestionIndex { 
        get 
        { 
            return questionIndex; 
        } 
        set 
        { 
            questionIndex = value; 
            questionTextObject.GetComponent<TextMeshProUGUI>().text = (questionIndex+1) + ". " + SjoelenMinigame.Instance.Questions[questionIndex];
            answerNumberObject.GetComponent<TextMeshProUGUI>().text = SjoelenMinigame.Instance.TryChangeAnswer(QuestionIndex, 0).ToString();
        } 
    }
    private int questionIndex;
    [SerializeField]
    private GameObject questionTextObject;
    [SerializeField]
    private GameObject answerNumberObject;

    public void Decrease()
    {
        answerNumberObject.GetComponent<TextMeshProUGUI>().text = SjoelenMinigame.Instance.TryChangeAnswer(QuestionIndex,-1).ToString();
    }
    public void Increase()
    {
        answerNumberObject.GetComponent<TextMeshProUGUI>().text = SjoelenMinigame.Instance.TryChangeAnswer(QuestionIndex, 1).ToString();
    }
}
