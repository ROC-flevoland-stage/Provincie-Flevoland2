using UnityEngine;

public class SortZone : MonoBehaviour
{
    // When an object with the Unity tag "File" enters this trigger, destroy it.
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("File"))
        {
            Destroy(other.gameObject);
        }
    }
}
