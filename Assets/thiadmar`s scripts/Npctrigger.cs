using UnityEngine;

public class Npctrigger : MonoBehaviour
{
    [Header("Instellingen")]
    [SerializeField] private bool usePortemonneeQuest = false;

    [Header("Algemene Dialogen")]
    [SerializeField] private DialogueTree normalDialogue;   // Standaard praatje
    [SerializeField] private DialogueTree thankYouDialogue; // Bedankje (voor beide quests)

    [Header("Portemonnee Quest")]
    [SerializeField] private DialogueTree portemonneeQuestDialogue;  // De vraag om hulp
    [SerializeField] private DialogueTree portemonneeReturnDialogue; // Bij inleveren

    private PlayerInventory currentInventory;

    private void Start()
    {
        // Callback voor de portemonnee quest
        DialogueVariables.Instance.RegisterCallback("portemonnee_teruggegeven", (v) => {
            if ((bool)v && currentInventory != null && currentInventory.HasItem)
                currentInventory.DropCurrent();
        });

        // Callback voor de oude quest
        DialogueVariables.Instance.RegisterCallback("wil je dit geven", (v) => {
            if ((bool)v && currentInventory != null && currentInventory.HasItem)
                currentInventory.DropCurrent();
        });
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        currentInventory = other.GetComponent<PlayerInventory>();
        if (currentInventory == null || DialogueManager.Instance == null) return;

        // Muis vrijgeven
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (usePortemonneeQuest) HandlePortemonneeQuest();
        else HandleOldQuest();
    }

    private void HandlePortemonneeQuest()
    {
        var dv = DialogueVariables.Instance;
        bool isTeruggegeven = dv.GetVariable<bool>("portemonnee_teruggegeven");

        // 1. Is de quest al klaar? Gebruik algemene bedank-dialoog
        if (isTeruggegeven)
        {
            DialogueManager.Instance.Startdialogue(thankYouDialogue);
        }
        // 2. Heeft de speler de portemonnee bij zich? Start teruggeef-dialoog
        else if (currentInventory.HasItem)
        {
            DialogueManager.Instance.Startdialogue(portemonneeReturnDialogue);
        }
        // 3. Speler heeft niets: Start de quest-vraag
        else
        {
            DialogueManager.Instance.Startdialogue(portemonneeQuestDialogue);
        }
    }

    private void HandleOldQuest()
    {
        // Werkt exact hetzelfde: item? -> bedanken, geen item? -> normaal
        if (currentInventory.HasItem)
            DialogueManager.Instance.Startdialogue(thankYouDialogue);
        else
            DialogueManager.Instance.Startdialogue(normalDialogue);
    }

}
