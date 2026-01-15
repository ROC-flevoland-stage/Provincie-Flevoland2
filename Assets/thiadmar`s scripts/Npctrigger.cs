using UnityEngine;

public class Npctrigger : MonoBehaviour
{
    [Header("Dialogue")]
    [SerializeField] private DialogueTree normalDialogue;
    [SerializeField] private DialogueTree thankYouDialogue;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerInventory inventory = other.GetComponent<PlayerInventory>();

        if (inventory != null && inventory.HasItem)
        {
            // Speler geeft item aan NPC
            inventory.DropCurrent();

            Debug.Log("NPC: Dankjewel voor het cadeau!");

            DialogueManager.Instance.Startdialogue(thankYouDialogue);
        }
        else
        {
            // Normale dialoog
            DialogueManager.Instance.Startdialogue(normalDialogue);
        }
    }
}
