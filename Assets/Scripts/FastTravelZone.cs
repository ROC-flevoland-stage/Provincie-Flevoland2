using UnityEngine;

public class FastTravelZone : MonoBehaviour
{
    [SerializeField] private GameObject fastTravelUI;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            fastTravelUI.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            fastTravelUI.SetActive(false);
    }
}
