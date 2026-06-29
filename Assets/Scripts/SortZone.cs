using UnityEngine;

public class SortZone : MonoBehaviour
{
    [Range(1, 5)]
    public int zoneNumber = 1;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("File")) return;

        Debug.Log("SortZone number: " + zoneNumber);
        Destroy(other.gameObject);
    }
}
