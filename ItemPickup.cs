using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] private string itemId = ItemIds.Apple;
    [SerializeField] private int amount = 1;

    public void Pickup()
    {
        QuestManager.Instance.Bus.RaiseItemCollected(itemId, amount);
        Destroy(gameObject);
    }
}
