// InventoryUI.cs ��ũ��Ʈ�� InventoryPanel�� ����
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    // �κ��丮 �Ŵ��� ����
    [SerializeField] private InventoryManager inventoryManager;

    // UI ��� ����
    [SerializeField] private Transform slotContainer;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private GameObject itemInfoPanel;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemDescriptionText;
    [SerializeField] private TextMeshProUGUI itemStatsText;

    // �κ��丮 ���� ����
    private List<InventorySlot> slots = new List<InventorySlot>();

    private void Start()
    {
        // �κ��丮 ���� �̺�Ʈ ����
        inventoryManager.OnInventoryChanged += UpdateUI;

        // �κ��丮 ���� �ʱ�ȭ
        InitializeInventorySlots(20); // ����: 20�� ���� ����

        // ���� �г� �ʱ� ����
        itemInfoPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        // �̺�Ʈ ���� ����
        if (inventoryManager != null)
        {
            inventoryManager.OnInventoryChanged -= UpdateUI;
        }
    }

    // �κ��丮 ���� �ʱ�ȭ
    private void InitializeInventorySlots(int slotCount)
    {
        // ���� ���� ����
        foreach (Transform child in slotContainer)
        {
            Destroy(child.gameObject);
        }
        slots.Clear();

        // �� ���� ����
        for (int i = 0; i < slotCount; i++)
        {
            GameObject slotGO = Instantiate(slotPrefab, slotContainer);
            InventorySlot slot = slotGO.GetComponent<InventorySlot>();
            slot.SlotIndex = i;
            slot.OnItemClicked += OnItemClicked;
            slot.OnItemPointerEnter += ShowItemInfo;
            slot.OnItemPointerExit += HideItemInfo;
            slots.Add(slot);
        }
    }

    // UI ������Ʈ
    private void UpdateUI(List<InventoryItem> inventory)
    {
        // ��� ���� �ʱ�ȭ
        foreach (InventorySlot slot in slots)
        {
            slot.ClearSlot();
        }

        // �κ��丮 ���������� ���� ä���
        for (int i = 0; i < inventory.Count; i++)
        {
            if (i < slots.Count)
            {
                slots[i].SetItem(inventory[i]);
            }
        }
    }

    // ������ Ŭ�� ó��
    private void OnItemClicked(InventoryItem item)
    {
        if (item != null)
        {
            inventoryManager.UseItem(item.item);
        }
    }

    // ������ ���� ǥ��
    private void ShowItemInfo(InventoryItem item, Vector2 position)
    {
        if (item != null)
        {
            itemNameText.text = item.item.itemName;
            itemDescriptionText.text = item.item.itemDescription;

            // ������ Ÿ�Կ� ���� ���� ǥ��
            string statsText = "";
            switch (item.item.itemType)
            {
                case Item.ItemType.Weapon:
                    statsText = $"���ݷ�: {item.item.damage}";
                    break;
                case Item.ItemType.Armor:
                    statsText = $"����: {item.item.defense}";
                    break;
                case Item.ItemType.Consumable:
                    statsText = $"ȸ����: {item.item.healAmount}";
                    break;
                default:
                    statsText = "";
                    break;
            }
            itemStatsText.text = statsText;

            // ���� �г� ��ġ �� ǥ��
            itemInfoPanel.transform.position = position;
            itemInfoPanel.SetActive(true);
        }
    }

    // ������ ���� �����
    private void HideItemInfo(InventoryItem item)
    {
        itemInfoPanel.SetActive(false);
    }
}