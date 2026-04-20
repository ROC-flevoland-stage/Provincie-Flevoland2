using UnityEngine;

public class SortZone : MonoBehaviour
{
    // Destroy de file als het komt in de sortzone.
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("File"))
        {
            Destroy(other.gameObject);
        }
    }
}
