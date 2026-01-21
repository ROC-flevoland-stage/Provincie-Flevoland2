using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public int score = 0;
    public int hp = 5;

    [Header("UI (TextMeshPro)")]
    public TMP_Text scoreText;
    public TMP_Text hpText;
    public GameObject gameOverPanel; // optineel voor later

    void Start()
    {
        UpdateUI();
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    public void CorrectFile(GameObject file)
    {
        score++;
        UpdateUI();
        Destroy(file);
    }

    public void WrongFile(GameObject file)
    {
        hp--;
        UpdateUI();
        Destroy(file);

        if (hp <= 0)
            GameOver();
    }

    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
        if (hpText != null)
            hpText.text = "HP: " + hp;
    }

    void GameOver()
    {
        Debug.Log("Game Over");
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        Time.timeScale = 0f;
    }
}
