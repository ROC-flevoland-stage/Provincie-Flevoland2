using UnityEngine;
using TMPro; // Required for TextMeshPro

public class TextTrigger : MonoBehaviour
{
    public GameObject textObject; 
    private void Start()
    {
        textObject.SetActive(false); 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            textObject.SetActive(true); 
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        { 
            textObject.SetActive(false); 
        
        }
    }

}