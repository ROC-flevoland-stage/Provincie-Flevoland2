using UnityEngine;

public class HoopAnswer : MonoBehaviour
{
    public int answerValue; // antwoord 1-10, ligt er aan welke hoop.

    public QuestionManager questionManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            Debug.Log("Scored in hoop: " + answerValue);

            questionManager.SubmitAnswer(answerValue);

        }
    }
}