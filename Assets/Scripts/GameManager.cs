using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int totalCubes = 10;
    public int cubesPlaced;
    int[] ratings;

    [Header("UI (TextMeshPro)")]
    public TMP_Text cubesLeftText;
    public TMP_Text resultText;

    bool gameEnded; 

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        ratings = new int[totalCubes];
        cubesPlaced = 0;
        gameEnded = false;

        if (resultText != null)
            resultText.gameObject.SetActive(false);

        UpdateUI();
    }

    // Sortzone called dit op met 1-5 rating
    public void PlaceCube(int rating, GameObject file)
    {
        if (gameEnded) return;

        rating = Mathf.Clamp(rating, 1, 5);
        if (cubesPlaced < totalCubes)
            ratings[cubesPlaced] = rating;

        cubesPlaced++;
        Destroy(file);
        UpdateUI();

        if (cubesPlaced >= totalCubes)
            EndGame();
    }

    // Update de UI met het aantal cubes dat nog geplaatst moet worden
    void UpdateUI()
    {
        if (cubesLeftText != null)
            cubesLeftText.text = "Cubes left: " + Mathf.Max(0, totalCubes - cubesPlaced);
    }
    // Einde van het spel, toont resultaten en pauzeert het spel
    void EndGame()
    {
        gameEnded = true;

        if (resultText != null)
        {
            resultText.gameObject.SetActive(true);
            resultText.text = "Game Over\nRatings: " + string.Join(", ", ratings);
        }

        Time.timeScale = 0f;
        SceneManager.LoadScene("E3Demo");
    }
}
