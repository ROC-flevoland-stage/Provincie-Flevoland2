using UnityEngine;
using UnityEngine.InputSystem;

public class PickupItem : MonoBehaviour
{
    public string itemName = "Key";
    public float pickupDistance = 3f;

    private Transform player;
    private bool pickedUp = false;

    private void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
            player = p.transform;
    }

    private void Update()
    {
        if (player == null || pickedUp) return;

        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= pickupDistance)
        {
            if (Keyboard.current == null) return;

            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                pickedUp = true;

                // Inventory
                PlayerInventory inventory = player.GetComponent<PlayerInventory>();
                if (inventory != null)
                    inventory.AddItem(itemName);

                // Quest progress
                if (QuestManager.Instance != null)
                    QuestManager.Instance.CollectItem(itemName, 1);

                Destroy(gameObject);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pickupDistance);
    }
}
