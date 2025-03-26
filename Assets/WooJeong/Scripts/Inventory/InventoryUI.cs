// InventoryUI.cs 스크립트를 InventoryPanel에 연결
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    // 인벤토리 매니저 참조
    [SerializeField] private InventoryManager inventoryManager;

    // UI 요소 참조
    [SerializeField] private Transform slotContainer;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private GameObject itemInfoPanel;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemDescriptionText;
    [SerializeField] private TextMeshProUGUI itemStatsText;

    // 인벤토리 슬롯 관리
    private List<InventorySlot> slots = new List<InventorySlot>();

    private void Start()
    {
        // 인벤토리 변경 이벤트 구독
        inventoryManager.OnInventoryChanged += UpdateUI;

        // 인벤토리 슬롯 초기화
        InitializeInventorySlots(20); // 예시: 20개 슬롯 생성

        // 정보 패널 초기 상태
        itemInfoPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제
        if (inventoryManager != null)
        {
            inventoryManager.OnInventoryChanged -= UpdateUI;
        }
    }

    // 인벤토리 슬롯 초기화
    private void InitializeInventorySlots(int slotCount)
    {
        // 기존 슬롯 정리
        foreach (Transform child in slotContainer)
        {
            Destroy(child.gameObject);
        }
        slots.Clear();

        // 새 슬롯 생성
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

    // UI 업데이트
    private void UpdateUI(List<InventoryItem> inventory)
    {
        // 모든 슬롯 초기화
        foreach (InventorySlot slot in slots)
        {
            slot.ClearSlot();
        }

        // 인벤토리 아이템으로 슬롯 채우기
        for (int i = 0; i < inventory.Count; i++)
        {
            if (i < slots.Count)
            {
                slots[i].SetItem(inventory[i]);
            }
        }
    }

    // 아이템 클릭 처리
    private void OnItemClicked(InventoryItem item)
    {
        if (item != null)
        {
            inventoryManager.UseItem(item.item);
        }
    }

    // 아이템 정보 표시
    private void ShowItemInfo(InventoryItem item, Vector2 position)
    {
        if (item != null)
        {
            itemNameText.text = item.item.itemName;
            itemDescriptionText.text = item.item.itemDescription;

            // 아이템 타입에 따른 스탯 표시
            string statsText = "";
            switch (item.item.itemType)
            {
                case Item.ItemType.Weapon:
                    statsText = $"공격력: {item.item.damage}";
                    break;
                case Item.ItemType.Armor:
                    statsText = $"방어력: {item.item.defense}";
                    break;
                case Item.ItemType.Consumable:
                    statsText = $"회복량: {item.item.healAmount}";
                    break;
                default:
                    statsText = "";
                    break;
            }
            itemStatsText.text = statsText;

            // 정보 패널 위치 및 표시
            itemInfoPanel.transform.position = position;
            itemInfoPanel.SetActive(true);
        }
    }

    // 아이템 정보 숨기기
    private void HideItemInfo(InventoryItem item)
    {
        itemInfoPanel.SetActive(false);
    }
}