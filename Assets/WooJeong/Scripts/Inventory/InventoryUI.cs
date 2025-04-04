using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Collections;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private InventoryManager inventoryManager;

    [SerializeField] private Transform slotContainer;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private GameObject itemInfoPanel;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemDescriptionText;
    [SerializeField] private TextMeshProUGUI itemStatsText;
    [SerializeField] private GameObject disposePanel;
    [SerializeField] private TMP_InputField disposeInput;
    [SerializeField] private Button confirmButton;

    private List<InventorySlot> slots = new List<InventorySlot>();
    private List<RectTransform> slotRectTransforms = new List<RectTransform>();

    private GraphicRaycaster raycaster;
    private PointerEventData pointerEventData;
    private List<RaycastResult> raycastResult = new();
    private InventorySlot curShowSlot;
    private Vector2 infoOffSet = new Vector2(300, 400);
    private RectTransform invenRect, infoRect;

    private Image dragItemImage;
    private InventorySlot curDragSlot;

    private bool isDoubleClick = false;
    private Coroutine coDoubleClick;

    private void Awake()
    {
        invenRect = GetComponent<RectTransform>();
        infoRect = itemInfoPanel.GetComponent<RectTransform>();
        pointerEventData = new PointerEventData(EventSystem.current);
        raycaster = itemInfoPanel.transform.parent.GetComponent<GraphicRaycaster>();
        itemInfoPanel.SetActive(false);
        confirmButton.onClick.AddListener(DisposeAndExit);
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
        if (dragItemImage == null)
        {
            Canvas canvas = FindObjectOfType<Canvas>();

            GameObject dragImageObj = new GameObject("DragItemImage");
            dragImageObj.transform.SetParent(canvas.transform);
            dragItemImage = dragImageObj.AddComponent<Image>();
            dragItemImage.raycastTarget = false;
            dragItemImage.rectTransform.sizeDelta = new Vector2(80, 80);

            dragItemImage.gameObject.SetActive(false);
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
        if (disposePanel.activeSelf == true)
            return;

        if (item.Item != null)
        {
            itemNameText.text = item.Item.ItemName;
            itemDescriptionText.text = item.Item.ItemDescription;

            string statsText = "";
            switch (item.Item.ItemType)
            {
                case Item.EItemType.Weapon:
                    statsText = $"공격력: {item.Item.Damage}";
                    break;
                case Item.EItemType.Armor:
                    statsText = $"방어력: {item.Item.Defense}";
                    break;
                case Item.EItemType.Consumable:
                    statsText = $"회복량: {item.Item.HealAmount}";
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
            if (curDragSlot == null && curShowSlot == null)
                return;

            if(curDragSlot == curShowSlot)
                CheckDoubleClick();
            else if (curDragSlot == null && curShowSlot.CurrentItem.Item != null) // 아이템 클릭
            {
                curDragSlot = curShowSlot;
                curDragSlot.IconImage.enabled = false;
                dragItemImage.sprite = curDragSlot.CurrentItem.Item.Icon;
                dragItemImage.gameObject.SetActive(true);
                dragItemImage.gameObject.transform.position = Input.mousePosition;
                if(coDoubleClick == null)
                {
                    coDoubleClick = StartCoroutine(DoubleClickTimer());
                }
                else
                {
                    StopCoroutine(coDoubleClick);
                    coDoubleClick = StartCoroutine(DoubleClickTimer());
                }    
            }
            else if(curDragSlot != null && curShowSlot != null) // 스왑
            {
                curDragSlot.IconImage.enabled = true;
                InventoryManager.Instance.SwapItems(curDragSlot.SlotIndex, curShowSlot.SlotIndex);
                curDragSlot = null;
                dragItemImage.gameObject.SetActive(false);
            }
            else if(curDragSlot != null && raycastResult.Count == 0) // 버리기
            {
                if(curDragSlot.CurrentItem.Quantity == 1)
                {
                    InventoryManager.Instance.RemoveItem(curDragSlot);
                    curDragSlot = null;
                    dragItemImage.gameObject.SetActive(false);
                }
                else
                {
                    SetDisposePanel();
                }
                Debug.Log("Click outside");                
            }
        }
    }

    private void OnItemDrag()
    {
        if(curDragSlot == null)
            return;

        dragItemImage.gameObject.transform.position = Input.mousePosition;
    }

    private void SetDisposePanel()
    {
        curDragSlot.IconImage.enabled = true;
        dragItemImage.gameObject.SetActive(false);

        disposePanel.SetActive(true);
        DOTween.Rewind("DisposeDisable");
        disposeInput.text = "";
    }

    private void DisposeAndExit()
    {
        if (int.TryParse(disposeInput.text.Trim(), out int n) == false)
        {
            Debug.Log(disposeInput.text);
            Debug.Log("text length " + disposeInput.text.Trim().Length);
            Debug.Log("parse fail");
            Debug.Log(n);
            foreach(var i in disposeInput.text.Trim())
            {
                Debug.Log(i);
            }
            curDragSlot = null;
            return;
        }
        InventoryManager.Instance.RemoveItem(curDragSlot, n);
        curDragSlot = null;

        DOTween.Rewind("DisposeDisable");
        DOTween.Play("DisposeDisable");
    }

    private bool CheckDoubleClick()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(curDragSlot != null && isDoubleClick == true)
            {
                Debug.Log($"UseItem {curDragSlot.CurrentItem.Item}");
                InventoryManager.Instance.RemoveItem(curDragSlot);
                curDragSlot = null;
                dragItemImage.gameObject.SetActive(false);
                //InventoryManager.Instance.UseItem(draggedFromSlot.CurrentItem.Item);
            }
        }
        return true;
    }

    private IEnumerator DoubleClickTimer()
    {
        isDoubleClick = true;
        yield return new WaitForSeconds(0.3f);
        isDoubleClick = false;
        coDoubleClick = null;
    }
}