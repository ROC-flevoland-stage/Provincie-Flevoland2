using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StamperGame : MonoBehaviour
{
    [Header("Paper")]
    public Image paperImage;
    public Sprite[] goodPaperSprites;
    public Sprite[] badPaperSprites;

    [Header("UI")]
    public TMP_Text timerText;
    public TMP_Text resultText;
    public TMP_Text papersStampedText;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip correctSound;
    public AudioClip wrongSound;

    [Header("Settings")]
    public float gameTime = 30f;
    public int papersToStampLimit = 7; 

    float timeLeft;
    bool isGoodPaper;
    bool gameOver = false;

    int papersStamped = 0;

    void Start()
    {
        timeLeft = gameTime;
        resultText.text = "";
        UpdateStampedText();
        SpawnPaper();
    }

    void Update()
    {
        if (gameOver) return;

        // Timer
        timeLeft -= Time.deltaTime;
        timerText.text = "Time: " + Mathf.Ceil(timeLeft);

        if (timeLeft <= 0)
        {
            EndGame("TIME UP");
        }

        // Input
        if (Input.GetMouseButtonDown(0)) // Accept
        {
            Stamp(true);
        }

        if (Input.GetMouseButtonDown(1)) // Reject
        {
            Stamp(false);
        }
    }

    void SpawnPaper()
    {
        isGoodPaper = Random.value > 0.5f;

        if (isGoodPaper && goodPaperSprites.Length > 0)
        {
            paperImage.sprite = goodPaperSprites[Random.Range(0, goodPaperSprites.Length)];
        }
        else if (!isGoodPaper && badPaperSprites.Length > 0)
        {
            paperImage.sprite = badPaperSprites[Random.Range(0, badPaperSprites.Length)];
        }
    }

    void Stamp(bool accepted)
    {
        papersStamped++;
        UpdateStampedText();

        // Feedback only
        if (accepted == isGoodPaper)
        {
            if (audioSource && correctSound)
                audioSource.PlayOneShot(correctSound);
        }
        else
        {
            if (audioSource && wrongSound)
                audioSource.PlayOneShot(wrongSound);
        }

        
        if (papersStamped >= papersToStampLimit)
        {
            EndGame("DONE");
            return;
        }

        SpawnPaper();
    }

    void UpdateStampedText()
    {
        papersStampedText.text =
            "Papers stamped: " + papersStamped + " / " + papersToStampLimit;
    }

    void EndGame(string endMessage)
    {
        gameOver = true;
        resultText.text = endMessage;
    }
}
