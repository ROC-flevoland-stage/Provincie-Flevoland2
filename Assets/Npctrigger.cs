using UnityEngine;

public class Npctrigger : MonoBehaviour 
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject);
        DialogueManager.Instance.Startdialogue(GetComponent<DialogueTree>());
    }
}

