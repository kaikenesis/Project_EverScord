// InventoryUI.cs 스크립트를 InventoryPanel에 연결
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using DG.Tweening;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private InventoryManager inventoryManager;

    [SerializeField] private Transform slotContainer;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private GameObject itemInfoPanel;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemDescriptionText;
    [SerializeField] private TextMeshProUGUI itemStatsText;

    private List<InventorySlot> slots = new List<InventorySlot>();
    private List<RectTransform> slotRectTransforms = new List<RectTransform>();

    private GraphicRaycaster raycaster;
    private PointerEventData pointerEventData;
    private List<RaycastResult> raycastResult = new();
    private InventorySlot curShowSlot;
    private Vector2 infoOffSet = new Vector2(300, 400);
    private RectTransform invenRect, infoRect;

    private Image draggedItemImage;
    private InventorySlot draggedFromSlot;

    private void Awake()
    {
        invenRect = GetComponent<RectTransform>();
        infoRect = itemInfoPanel.GetComponent<RectTransform>();
        pointerEventData = new PointerEventData(EventSystem.current);
        raycaster = itemInfoPanel.transform.parent.GetComponent<GraphicRaycaster>();
        itemInfoPanel.SetActive(false);
        MakeDragItem();
    }

    private void Start()
    {
        inventoryManager.OnInventoryChanged += UpdateUI;
        InitializeInventorySlots(InventoryManager.Instance.InventoryCapacity);        
    }

    private void Update()
    {
        pointerEventData.position = Input.mousePosition;

        OnPointerSlotEnter();
        OnItemClicked();
        OnItemDrag();
    }

    private void OnDisable()
    {
        itemInfoPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        if (inventoryManager != null)
        {
            inventoryManager.OnInventoryChanged -= UpdateUI;
        }
    }

    private void MakeDragItem()
    {
        if (draggedItemImage == null)
        {
            Canvas canvas = FindObjectOfType<Canvas>();

            GameObject dragImageObj = new GameObject("DragItemImage");
            dragImageObj.transform.SetParent(canvas.transform);
            draggedItemImage = dragImageObj.AddComponent<Image>();
            draggedItemImage.raycastTarget = false;
            draggedItemImage.rectTransform.sizeDelta = new Vector2(80, 80);

            draggedItemImage.gameObject.SetActive(false);
        }
    }

    private void InitializeInventorySlots(int slotCount)
    {
        foreach (Transform child in slotContainer)
        {
            Destroy(child.gameObject);
        }
        slots.Clear();

        for (int i = 0; i < slotCount; i++)
        {
            GameObject slotGO = Instantiate(slotPrefab, slotContainer);
            InventorySlot slot = slotGO.GetComponent<InventorySlot>();
            slot.SlotIndex = i;
            slots.Add(slot);
        }
    }

    private void UpdateUI(List<InventoryItem> inventory)
    {
        foreach (InventorySlot slot in slots)
        {
            slot.ClearSlot();
        }

        for (int i = 0; i < inventory.Count; i++)
        {
            if (i < slots.Count)
            {
                slots[i].SetItem(inventory[i]);
            }
        }
    }

    private void DecideInventoryOffset()
    {
        if(Screen.currentResolution.width / 2 >= transform.position.x)
        {
            infoOffSet = new Vector2(invenRect.rect.width / 2, invenRect.rect.height / 2);
        }
        else
        {
            infoOffSet = new Vector2((invenRect.rect.width / 2 + infoRect.rect.width) * -1, invenRect.rect.height / 2);
        }
    }

    private void ShowItemInfo(InventoryItem item)
    {
        if (item.item != null)
        {
            itemNameText.text = item.item.itemName;
            itemDescriptionText.text = item.item.itemDescription;

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

            DecideInventoryOffset();
            itemInfoPanel.transform.position = (Vector2)transform.position + infoOffSet;
            itemInfoPanel.SetActive(true);
        }
    }

    private void HideItemInfo()
    {
        itemInfoPanel.SetActive(false);
    }

    private T RaycastAndTryGetComponent<T>() where T : Component
    {
        raycastResult.Clear();

        raycaster.Raycast(pointerEventData, raycastResult);

        if (raycastResult.Count == 0)
            return null;

        return raycastResult[0].gameObject.GetComponent<T>();
    }

    public void OnPointerSlotEnter()
    {
        InventorySlot slot = RaycastAndTryGetComponent<InventorySlot>();
        
        if (slot == null)
        {
            HideItemInfo();
            curShowSlot = null;
            return;
        }

        curShowSlot = slot;
        ShowItemInfo(slot.CurrentItem);
    }

    private void OnItemClicked()
    {
        if(Input.GetMouseButtonDown(0))
        {
            // 처음 아이템을 선택
            if (draggedFromSlot == null && curShowSlot != null)
            {
                draggedFromSlot = curShowSlot;
                draggedFromSlot.IconImage.enabled = false;
                draggedItemImage.sprite = draggedFromSlot.CurrentItem.item.icon;
                draggedItemImage.gameObject.SetActive(true);
                draggedItemImage.gameObject.transform.position = Input.mousePosition;
            }
            // 다른 아이템을 선택하고 스왑
            else if(draggedFromSlot != null && curShowSlot != null)
            {
                draggedFromSlot.IconImage.enabled = true;
                InventoryManager.Instance.SwapItems(draggedFromSlot.SlotIndex, curShowSlot.SlotIndex);
                draggedFromSlot = null;
                draggedItemImage.gameObject.SetActive(false);
            }
            // 밖을 선택
            else if(draggedFromSlot != null && raycastResult.Count == 0)
            {
                Debug.Log("Click outside");
                InventoryManager.Instance.RemoveItem(draggedFromSlot);
                draggedFromSlot = null;
                draggedItemImage.gameObject.SetActive(false);
            }
        }
    }

    private void OnItemDrag()
    {
        if(draggedFromSlot == null)
            return;

        draggedItemImage.gameObject.transform.position = Input.mousePosition;
    }
}