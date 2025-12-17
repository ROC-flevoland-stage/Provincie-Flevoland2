using UnityEngine;

public class NPCController : MonoBehaviour
{
    [Header("NPC Settings")]
    [SerializeField] private string npcId = "NPC_1";
    [SerializeField] private Transform player;
    [SerializeField] private float talkDistance = 3f;

    private void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= talkDistance)
        {
            HandleInteraction();
        }
    }

    private void HandleInteraction()
    {
        // New Input System
        if (UnityEngine.InputSystem.Keyboard.current == null) return;
        if (!UnityEngine.InputSystem.Keyboard.current.eKey.wasPressedThisFrame) return;

        if (QuestManager.Instance == null)
        {
            Debug.LogWarning("QuestManager niet gevonden in de scene!");
            return;
        }

        QuestManager.Instance.InteractWithNpc(npcId);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, talkDistance);
    }
}
