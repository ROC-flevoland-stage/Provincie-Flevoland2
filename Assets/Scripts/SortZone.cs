using UnityEngine;

public class SortZone : MonoBehaviour
{
<<<<<<< Updated upstream
    public int rating = 1;
    public GameManager gameManager;
=======
    
    public int zoneNumber = 1;
>>>>>>> Stashed changes

    // Destroy de file als het komt in de sortzone.
    void OnTriggerEnter(Collider other)
    {
<<<<<<< Updated upstream
        if (other.CompareTag("File"))
        {
            gameManager.PlaceCube(rating, other.gameObject);
        }
=======
        if (!other.CompareTag("File")) return;

        Debug.Log("SortZone number: " + zoneNumber);
        Destroy(other.gameObject);
>>>>>>> Stashed changes
    }
}
