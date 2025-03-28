using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItem
{
    public Item Item;
    public int Quantity;

    public InventoryItem(Item item, int quantity)
    {
        Item = item;
        Quantity = quantity;
    }
}
