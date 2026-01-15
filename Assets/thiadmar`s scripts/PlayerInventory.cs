using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private PickupItem currentItem;

    public bool HasItem => currentItem != null;

    public bool TryStore(PickupItem item, Transform holdPoint)
    {
        if (item == null) return false;
        if (currentItem != null) return false;

        currentItem = item;
        currentItem.OnPickedUp(holdPoint);
        return true;
    }

    public void DropCurrent()
    {
        if (currentItem == null) return;

        currentItem.OnDropped();
        currentItem = null;
    }
}
