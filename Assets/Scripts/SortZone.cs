using UnityEngine;

public class SortZone : MonoBehaviour
{
    public int rating = 1;
    public GameManager gameManager;

    // Destroy de file als het komt in de sortzone.
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("File"))
        {
            gameManager.PlaceCube(rating, other.gameObject);
        }
    }
}
