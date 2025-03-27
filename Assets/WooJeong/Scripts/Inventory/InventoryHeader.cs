using DG.Tweening;
using EverScord;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryHeader : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    private GameObject inventoryPanel;
    private Button exitButton;
    private Vector2 offSet;

    private void Awake()
    {
        inventoryPanel = transform.parent.gameObject;
        exitButton = GetComponentInChildren<Button>();
        exitButton.onClick.AddListener(Exit);
    }

    public void Exit()
    {
        DOTween.Rewind(ConstStrings.TWEEN_INVEN_DISABLE);
        DOTween.Play(ConstStrings.TWEEN_INVEN_DISABLE);
    }

    public void OnDrag(PointerEventData eventData)
    {
        inventoryPanel.transform.position = eventData.position + offSet;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        offSet = new Vector2(inventoryPanel.transform.position.x - eventData.position.x,
                            inventoryPanel.transform.position.y - eventData.position.y);
    }
}
