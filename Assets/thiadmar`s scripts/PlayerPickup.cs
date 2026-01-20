using UnityEngine;

public class PlayerPickupController : MonoBehaviour
{
    [SerializeField] private Transform holdPoint;
    [SerializeField] private PlayerInventory inventory;

    private PickupItem itemInRange;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (itemInRange != null && !inventory.HasItem)
                inventory.TryStore(itemInRange, holdPoint);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            inventory.DropCurrent();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PickupItem item = other.GetComponent<PickupItem>();
        if (item != null) itemInRange = item;
    }

    private void OnTriggerExit(Collider other)
    {
        PickupItem item = other.GetComponent<PickupItem>();
        if (item != null && item == itemInRange) itemInRange = null;
    }
}
