using UnityEngine;



public class Itemtrigger : MonoBehaviour
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
            "wil je dit item houden",
            false,
            (v) =>
            {
                if ((bool)v)
                {
                    // STOP de trigger: schakel de collider uit zodat deze niet meer afgaat
                    GetComponent<Collider>().enabled = false;
                    Debug.Log("Trigger gedeactiveerd omdat item is gegeven.");
                }
            }
        );

        if (inventory != null && inventory.HasItem)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            DialogueManager.Instance.Startdialogue(normalDialogue);
        }
    }
}
