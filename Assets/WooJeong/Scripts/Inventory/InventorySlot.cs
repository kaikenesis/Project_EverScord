// InventorySlot.cs ��ũ��Ʈ
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;

public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI quantityText;
    [SerializeField] private GameObject quantityContainer;

    private InventoryItem currentItem;
    private static Image draggedItemImage;
    private static InventorySlot draggedFromSlot;

    // ���� �ε���
    public int SlotIndex { get; set; }

    // �̺�Ʈ ��������Ʈ
    public event Action<InventoryItem> OnItemClicked;
    public event Action<InventoryItem, Vector2> OnItemPointerEnter;
    public event Action<InventoryItem> OnItemPointerExit;

    private void Awake()
    {
        // ó�� ���� �� �巡�� �̹����� ������ ����
        if (draggedItemImage == null)
        {
            // Canvas ã��
            Canvas canvas = FindObjectOfType<Canvas>();

            // DragItemImage ���� ������Ʈ ã�� �Ǵ� ����
            GameObject dragImageObj = GameObject.Find("DragItemImage");
            if (dragImageObj == null)
            {
                dragImageObj = new GameObject("DragItemImage");
                dragImageObj.transform.SetParent(canvas.transform);
                draggedItemImage = dragImageObj.AddComponent<Image>();
                draggedItemImage.raycastTarget = false;
            }
            else
            {
                draggedItemImage = dragImageObj.GetComponent<Image>();
            }

            // �ʱ� ���·� ��Ȱ��ȭ
            draggedItemImage.gameObject.SetActive(false);
        }
    }

    public void SetItem(InventoryItem inventoryItem)
    {
        currentItem = inventoryItem;

        // ������ ����
        if (iconImage != null)
        {
            iconImage.sprite = inventoryItem.item.icon;
            iconImage.enabled = true;
        }

        // ���� ����
        if (quantityText != null && quantityContainer != null)
        {
            if (inventoryItem.quantity > 1)
            {
                quantityText.text = inventoryItem.quantity.ToString();
                quantityContainer.SetActive(true);
            }
            else
            {
                quantityContainer.SetActive(false);
            }
        }
    }

    public void ClearSlot()
    {
        currentItem = null;

        if (iconImage != null)
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
        }

        if (quantityContainer != null)
        {
            quantityContainer.SetActive(false);
        }
    }

    // Ŭ�� �̺�Ʈ
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Click");
        if (currentItem != null)
        {
            OnItemClicked?.Invoke(currentItem);
        }
    }

    // ���콺 ���� �̺�Ʈ
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentItem != null)
        {
            OnItemPointerEnter?.Invoke(currentItem, eventData.position);
        }
    }

    // ���콺 ��Ż �̺�Ʈ
    public void OnPointerExit(PointerEventData eventData)
    {
        if (currentItem != null)
        {
            OnItemPointerExit?.Invoke(currentItem);
        }
    }

    // �巡�� ����
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (currentItem != null)
        {
            draggedFromSlot = this;
            draggedItemImage.sprite = iconImage.sprite;
            draggedItemImage.rectTransform.sizeDelta = new Vector2(80, 80); // �巡�� �̹��� ũ��
            draggedItemImage.gameObject.SetActive(true);
        }
    }

    // �巡�� ��
    public void OnDrag(PointerEventData eventData)
    {
        if (currentItem != null)
        {
            draggedItemImage.transform.position = eventData.position;
        }
    }

    // �巡�� ����
    public void OnEndDrag(PointerEventData eventData)
    {
        draggedFromSlot = null;
        draggedItemImage.gameObject.SetActive(false);
    }

    // ������ ���
    public void OnDrop(PointerEventData eventData)
    {
        if (draggedFromSlot != null && draggedFromSlot != this)
        {
            // �κ��丮 �Ŵ����� ���� ������ ��ġ ��ȯ ó��
            InventoryManager.Instance.SwapItems(draggedFromSlot.SlotIndex, SlotIndex);
        }
    }
}