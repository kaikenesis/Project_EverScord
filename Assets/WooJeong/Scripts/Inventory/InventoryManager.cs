using EverScord;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{
    public event Action<List<InventoryItem>> OnInventoryChanged;
    [SerializeField] private List<InventoryItem> inventory = new List<InventoryItem>();
    [SerializeField] private int inventoryCapacity = 20;

    public void SwapItems(int fromIndex, int toIndex)
    {
        if (fromIndex >= 0 && fromIndex < inventory.Count && toIndex >= 0 && toIndex < inventory.Count)
        {
            // ������ ��ȯ
            (inventory[toIndex], inventory[fromIndex]) = (inventory[fromIndex], inventory[toIndex]);

            // UI ������Ʈ �̺�Ʈ �߻�
            OnInventoryChanged?.Invoke(inventory);
        }
    }

    public bool AddItem(Item item, int quantity = 1)
    {
        if (item == null)
            return false;

        if (inventory.Count >= inventoryCapacity && !HasItem(item))
        {
            Debug.Log("�κ��丮�� ���� á���ϴ�!");
            return false;
        }

        if (item.isStackable)
        {
            foreach (InventoryItem inventoryItem in inventory)
            {
                if (inventoryItem.item == item)
                {
                    if (inventoryItem.quantity + quantity <= item.maxStackSize)
                    {
                        inventoryItem.quantity += quantity;
                        OnInventoryChanged?.Invoke(inventory);
                        return true;
                    }
                    else if (inventoryItem.quantity < item.maxStackSize)
                    {
                        int remainingSpace = item.maxStackSize - inventoryItem.quantity;
                        inventoryItem.quantity = item.maxStackSize;
                        return AddItem(item, quantity - remainingSpace);
                    }
                }
            }
        }

        if (inventory.Count < inventoryCapacity)
        {
            InventoryItem newItem = new InventoryItem(item, Mathf.Min(quantity, item.maxStackSize));
            inventory.Add(newItem);

            if (item.isStackable && quantity > item.maxStackSize)
            {
                OnInventoryChanged?.Invoke(inventory);
                return AddItem(item, quantity - item.maxStackSize);
            }

            OnInventoryChanged?.Invoke(inventory);
            return true;
        }

        Debug.Log("�κ��丮�� �������� �߰��� �� �����ϴ�!");
        return false;
    }

    public bool RemoveItem(Item item, int quantity = 1)
    {
        if (item == null)
            return false;

        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].item == item)
            {
                if (inventory[i].quantity > quantity)
                {
                    inventory[i].quantity -= quantity;
                    OnInventoryChanged?.Invoke(inventory);
                    return true;
                }
                else if (inventory[i].quantity == quantity)
                {
                    inventory.RemoveAt(i);
                    OnInventoryChanged?.Invoke(inventory);
                    return true;
                }
                else
                {
                    int removedQuantity = inventory[i].quantity;
                    inventory.RemoveAt(i);
                    OnInventoryChanged?.Invoke(inventory);
                    return RemoveItem(item, quantity - removedQuantity);
                }
            }
        }

        Debug.Log("�κ��丮���� �������� ã�� �� �����ϴ�!");
        return false;
    }

    public bool UseItem(Item item)
    {
        if (HasItem(item))
        {
            // ������ ��� ������ ���⿡ �ۼ�
            // ������ ������ ���� �ٸ� ȿ�� �ߵ� ����

            // �Ҹ�ǰ�� ��� �κ��丮���� ����
            if (item.itemType == Item.ItemType.Consumable)
            {
                return RemoveItem(item, 1);
            }

            return true;
        }

        return false;
    }

    public bool HasItem(Item item)
    {
        foreach (InventoryItem inventoryItem in inventory)
        {
            if (inventoryItem.item == item)
            {
                return true;
            }
        }

        return false;
    }

    public int GetItemQuantity(Item item)
    {
        int quantity = 0;

        foreach (InventoryItem inventoryItem in inventory)
        {
            if (inventoryItem.item == item)
            {
                quantity += inventoryItem.quantity;
            }
        }

        return quantity;
    }

    public List<InventoryItem> GetInventory()
    {
        return inventory;
    }

    public void ClearInventory()
    {
        inventory.Clear();
        OnInventoryChanged?.Invoke(inventory);
    }

}