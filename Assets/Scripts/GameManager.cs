using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public int totalCubes = 10;
    int cubesPlaced;
    int[] ratings;

    [Header("UI (TextMeshPro)")]
    public TMP_Text cubesLeftText;
    public TMP_Text resultText;

    bool gameEnded; 

    void Start()
    {
        ratings = new int[totalCubes];
        cubesPlaced = 0;
        gameEnded = false;

        if (resultText != null)
            resultText.gameObject.SetActive(false);

        UpdateUI();
    }

    // Called by SortZone with ratingValue (1-5)
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

    // Backwards-compatible helpers (optional mapping)
    public void CorrectFile(GameObject file)
    {
        PlaceCube(5, file);
    }

    public void WrongFile(GameObject file)
    {
        PlaceCube(1, file);
    }

    void UpdateUI()
    {
        if (cubesLeftText != null)
            cubesLeftText.text = "Cubes left: " + Mathf.Max(0, totalCubes - cubesPlaced);
    }

    void EndGame()
    {
        gameEnded = true;

        if (resultText != null)
        {
            resultText.gameObject.SetActive(true);
            resultText.text = "Game Over\nRatings: " + string.Join(", ", ratings);
        }

        Time.timeScale = 0f;
    }
}
