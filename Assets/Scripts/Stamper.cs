using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StamperGame : MonoBehaviour
{
    [Header("Paper")]
    public Image paperImage;
    public Sprite[] paperSprites;   

    [Header("UI")]
    public TMP_Text timerText;
    public TMP_Text resultText;
    public TMP_Text NewsratedText;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip stampSound;

    [Header("Settings")]
    public float gameTime = 30f;
    public int papersToStampLimit = 7;

    // YES / NO LIMIT
    public int yesLimit = 5;
    public int noLimit = 5;

    float timeLeft;
    bool gameOver = false;

    int papersStamped = 0;
    int yesCount = 0;
    int noCount = 0;

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
        if (Input.GetMouseButtonDown(0) && yesCount < yesLimit) // YES
        {
            yesCount++;
            Stamp();
        }

        if (Input.GetMouseButtonDown(1) && noCount < noLimit) // NO
        {
            noCount++;
            Stamp();
        }
    }

    void SpawnPaper()
    {
        if (paperSprites.Length > 0)
        {
            paperImage.sprite = paperSprites[Random.Range(0, paperSprites.Length)];
        }
    }

    void Stamp()
    {
        papersStamped++;
        UpdateStampedText();

        if (audioSource && stampSound)
            audioSource.PlayOneShot(stampSound);

        if (papersStamped >= papersToStampLimit)
        {
            EndGame("DONE");
            return;
        }

        SpawnPaper();
    }

    void UpdateStampedText()
    {
        NewsratedText.text =
            "News Rated: " + papersStamped + " / " + papersToStampLimit;
    }

    void EndGame(string endMessage)
    {
        gameOver = true;
        resultText.text = endMessage;
    }
}
