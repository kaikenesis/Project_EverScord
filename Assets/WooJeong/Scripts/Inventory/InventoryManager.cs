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
            inventory.Add(new InventoryItem(null, 0));
        }
    }

    private void Start()
    {
        OnInventoryChanged?.Invoke(inventory);
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
        if (fromIndex == toIndex)
            return;

        if (fromIndex >= 0 && fromIndex < inventoryCapacity && toIndex >= 0 && toIndex < inventoryCapacity)
        {
            if (inventory[fromIndex].Item == inventory[toIndex].Item && inventory[fromIndex].Item.IsStackable)
            {
                int maxSize = inventory[fromIndex].Item.MaxStackSize;

                if (inventory[toIndex].Quantity + inventory[fromIndex].Quantity <= maxSize)
                {
                    inventory[toIndex].Quantity = inventory[toIndex].Item.MaxStackSize;
                    inventory[fromIndex].Item = null;
                    inventory[fromIndex].Quantity = 0;
                }
                else if (inventory[toIndex].Quantity + inventory[fromIndex].Quantity > maxSize)
                {
                    int need = maxSize - inventory[toIndex].Quantity;
                    inventory[toIndex].Quantity = maxSize;
                    inventory[fromIndex].Quantity -= need;
                }
            }
            else
            {
                (inventory[toIndex], inventory[fromIndex]) = (inventory[fromIndex], inventory[toIndex]);
            }
            OnInventoryChanged?.Invoke(inventory);
        }
    }

    private int GetBlankIndex()
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].Item == null)
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
            if(item.IsStackable == false)
                return false;
            foreach (var inventoryItem in inventory)
            {
                if (inventoryItem.Quantity + quantity > item.MaxStackSize)
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

        if (item.IsStackable)
        {
            foreach (var inventoryItem in inventory)
            {
                if (inventoryItem.Item == item)
                {
                    if (inventoryItem.Quantity + quantity <= item.MaxStackSize)
                    {
                        inventoryItem.Quantity += quantity;
                        OnInventoryChanged?.Invoke(inventory);
                        return true;
                    }
                    else if (inventoryItem.Quantity < item.MaxStackSize)
                    {
                        int remainingSpace = item.MaxStackSize - inventoryItem.Quantity;
                        inventoryItem.Quantity = item.MaxStackSize;
                        return AddItem(item, quantity - remainingSpace);
                    }
                }
            }
        }

        InventoryItem newItem = new InventoryItem(item, Mathf.Min(quantity, item.MaxStackSize));
        inventory[index] = newItem;

        if (item.IsStackable && quantity > item.MaxStackSize)
        {
            OnInventoryChanged?.Invoke(inventory);
            return AddItem(item, quantity - item.MaxStackSize);
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
            if (inventory[i].Item == item)
            {
                if (inventory[i].Quantity > quantity)
                {
                    inventory[i].Quantity -= quantity;
                    OnInventoryChanged?.Invoke(inventory);
                    return true;
                }
                else if (inventory[i].Quantity == quantity)
                {
                    inventory[i].Item = null;
                    inventory[i].Quantity = 0;
                    OnInventoryChanged?.Invoke(inventory);
                    return true;
                }
                else
                {
                    int removedQuantity = inventory[i].Quantity;
                    inventory.RemoveAt(i);
                    OnInventoryChanged?.Invoke(inventory);
                    return RemoveItem(item, quantity - removedQuantity);
                }
            }
        }

        Debug.Log("인벤토리에서 아이템을 찾을 수 없습니다!");
        return false;
    }

    public bool RemoveItem(InventorySlot slot, int quantity = 1)
    {
        if (inventory[slot.SlotIndex].Quantity > quantity)
        {
            inventory[slot.SlotIndex].Quantity -= quantity;
            OnInventoryChanged?.Invoke(inventory);
            return true;
        }
        else if (inventory[slot.SlotIndex].Quantity <= quantity)
        {
            inventory[slot.SlotIndex].Item = null;
            inventory[slot.SlotIndex].Quantity = 0;
            OnInventoryChanged?.Invoke(inventory);
            return true;
        }
        
        return false;
    }


    public bool UseItem(Item item)
    {
        if (HasItem(item))
        {
            // 아이템 사용 구현은 여기에 작성
            // 아이템 유형에 따라 다른 효과 발동 가능

            // 소모품의 경우 인벤토리에서 제거
            if (item.ItemType == Item.EItemType.Consumable)
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
            if (inventoryItem.Item == item)
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
            if (inventoryItem.Item == item)
            {
                quantity += inventoryItem.Quantity;
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