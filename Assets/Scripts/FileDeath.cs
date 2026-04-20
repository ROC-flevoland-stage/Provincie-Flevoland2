using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FileDeath : MonoBehaviour
{
    public GameManager gameManager;
    public bool requireTag = false;
    public string requiredTag = "NotGood";

    void OnTriggerEnter(Collider other)
    { //File met SpawnedFile component is een echte file, dus vernietig het en update de game manager
        var file = other.GetComponent<SpawnedFile>();
        if (file == null) return;
        if (requireTag && !other.CompareTag(requiredTag)) return;

        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null) return;

        gameManager.WrongFile(other.gameObject);
    }
}