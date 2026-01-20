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
        DialogueVariables.Instance.CreateVariable<bool>(
            "wil je dit geven",
            false,
            (v) =>
            {
                if ((bool)v) 
                    // Speler geeft item aan NPC
                    inventory.DropCurrent();
            }            
            );

        if (inventory != null && inventory.HasItem)
        {

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

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
