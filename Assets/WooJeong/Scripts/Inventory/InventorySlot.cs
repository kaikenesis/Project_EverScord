// InventorySlot.cs 스크립트
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

    // 슬롯 인덱스
    public int SlotIndex { get; set; }

    // 이벤트 델리게이트
    public event Action<InventoryItem> OnItemClicked;
    public event Action<InventoryItem, Vector2> OnItemPointerEnter;
    public event Action<InventoryItem> OnItemPointerExit;

    private void Awake()
    {
        // 처음 시작 시 드래그 이미지가 없으면 생성
        if (draggedItemImage == null)
        {
            // Canvas 찾기
            Canvas canvas = FindObjectOfType<Canvas>();

            // DragItemImage 게임 오브젝트 찾기 또는 생성
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

            // 초기 상태로 비활성화
            draggedItemImage.gameObject.SetActive(false);
        }
    }

    public void SetItem(InventoryItem inventoryItem)
    {
        currentItem = inventoryItem;

        // 아이콘 설정
        if (iconImage != null)
        {
            iconImage.sprite = inventoryItem.item.icon;
            iconImage.enabled = true;
        }

        // 수량 설정
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

    // 클릭 이벤트
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Click");
        if (currentItem != null)
        {
            OnItemClicked?.Invoke(currentItem);
        }
    }

    // 마우스 엔터 이벤트
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentItem != null)
        {
            OnItemPointerEnter?.Invoke(currentItem, eventData.position);
        }
    }

    // 마우스 이탈 이벤트
    public void OnPointerExit(PointerEventData eventData)
    {
        if (currentItem != null)
        {
            OnItemPointerExit?.Invoke(currentItem);
        }
    }

    // 드래그 시작
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (currentItem != null)
        {
            draggedFromSlot = this;
            draggedItemImage.sprite = iconImage.sprite;
            draggedItemImage.rectTransform.sizeDelta = new Vector2(80, 80); // 드래그 이미지 크기
            draggedItemImage.gameObject.SetActive(true);
        }
    }

    // 드래그 중
    public void OnDrag(PointerEventData eventData)
    {
        if (currentItem != null)
        {
            draggedItemImage.transform.position = eventData.position;
        }
    }

    // 드래그 종료
    public void OnEndDrag(PointerEventData eventData)
    {
        draggedFromSlot = null;
        draggedItemImage.gameObject.SetActive(false);
    }

    // 아이템 드롭
    public void OnDrop(PointerEventData eventData)
    {
        if (draggedFromSlot != null && draggedFromSlot != this)
        {
            // 인벤토리 매니저를 통해 아이템 위치 교환 처리
            InventoryManager.Instance.SwapItems(draggedFromSlot.SlotIndex, SlotIndex);
        }
    }
}