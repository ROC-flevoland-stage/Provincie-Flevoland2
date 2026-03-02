using UnityEngine;

public class SortZone : MonoBehaviour
{
    public int ratingValue = 1; // Set this in the Inspector for each zone (1-5)
    public GameManager gameManager;

    void OnTriggerEnter(Collider other)
    {
        var file = other.GetComponent<SpawnedFile>();
        if (file == null) return;

        if (gameManager != null)
        {
            gameManager.PlaceCube(ratingValue, other.gameObject);
        }
    }
}
