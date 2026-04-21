using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FileDeath : MonoBehaviour
{
    public GameManager gameManager;
    public bool requireTag = false;
    public string requiredTag = "NotGood";

    void OnTriggerEnter(Collider other)
    {
        var file = other.GetComponent<File>();
        if (file == null) return;
        if (requireTag && !other.CompareTag(requiredTag)) return;

        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null) return;

        gameManager.WrongFile(other.gameObject);
    }
}