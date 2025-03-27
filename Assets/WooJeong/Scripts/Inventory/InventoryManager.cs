using DG.Tweening;
using EverScord;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : Singleton<InventoryManager>
{
    public event Action<List<InventoryItem>> OnInventoryChanged;
    [SerializeField] private List<InventoryItem> inventory = new List<InventoryItem>();
    [SerializeField] private int inventoryCapacity = 30;
    public int InventoryCapacity => inventoryCapacity;

    [SerializeField] private GameObject inventoryPanel;

    [SerializeField] private Item item;
    [SerializeField] private Item item2;

    protected override void Awake()
    {
        base.Awake();
        for(int i = 0; i < inventoryCapacity; i++)
        {
            inventory.Add(new InventoryItem(null, 1));
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            AddItem(item);        
        if (Input.GetKeyDown(KeyCode.S))
            AddItem(item2);
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (inventoryPanel.activeSelf == true)
            {
                DOTween.Rewind(ConstStrings.TWEEN_INVEN_DISABLE);
                DOTween.Play(ConstStrings.TWEEN_INVEN_DISABLE);
            }
            else
            {
                inventoryPanel.SetActive(true);
                DOTween.Rewind(ConstStrings.TWEEN_INVEN_DISABLE);
            }
        }
    }

    public void SwapItems(int fromIndex, int toIndex)
    {
        if (fromIndex >= 0 && fromIndex < inventoryCapacity && toIndex >= 0 && toIndex < inventoryCapacity)
        {
            (inventory[toIndex], inventory[fromIndex]) = (inventory[fromIndex], inventory[toIndex]);
            OnInventoryChanged?.Invoke(inventory);
        }
    }

    private int GetBlankIndex()
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].item == null)
                return i;
        }
        return -1;
    }

    public bool AddItem(Item item, int quantity = 1)
    {
        if (item == null)
            return false;
        int index = GetBlankIndex();

        if (index == -1 && HasItem(item) == true)
        {
            if(item.isStackable == false)
                return false;
            foreach (var inventoryItem in inventory)
            {
                if (inventoryItem.quantity + quantity > item.maxStackSize)
                {
                    return false;
                }
            }
        }
        else if (index == -1)
        {
            Debug.Log("인벤토리가 가득 찼습니다!");
            return false;
        }

        if (item.isStackable)
        {
            foreach (var inventoryItem in inventory)
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

        InventoryItem newItem = new InventoryItem(item, Mathf.Min(quantity, item.maxStackSize));
        inventory[index] = newItem;

        if (item.isStackable && quantity > item.maxStackSize)
        {
            OnInventoryChanged?.Invoke(inventory);
            return AddItem(item, quantity - item.maxStackSize);
        }

        OnInventoryChanged?.Invoke(inventory);
        return true;
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

        Debug.Log("인벤토리에서 아이템을 찾을 수 없습니다!");
        return false;
    }

    public bool UseItem(Item item)
    {
        if (HasItem(item))
        {
            // 아이템 사용 구현은 여기에 작성
            // 아이템 유형에 따라 다른 효과 발동 가능

            // 소모품의 경우 인벤토리에서 제거
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