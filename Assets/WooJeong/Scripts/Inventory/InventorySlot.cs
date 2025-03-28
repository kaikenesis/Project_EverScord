// InventorySlot.cs 스크립트
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;
using System.Collections.Generic;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private GameObject iconContainer;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI quantityText;
    [SerializeField] private GameObject quantityContainer;

    public InventoryItem CurrentItem { get; private set; }
    public Image IconImage => iconImage;
    public int SlotIndex;

    public void SetItem(InventoryItem inventoryItem)
    {
        CurrentItem = inventoryItem;

        if (CurrentItem.Item != null)
        {
            iconImage.sprite = inventoryItem.Item.Icon;
            iconImage.enabled = true;
        }
        else
        {
            iconImage.enabled = false;
        }

        if (quantityText != null && quantityContainer != null)
        {
            if (inventoryItem.Quantity > 1)
            {
                quantityText.text = inventoryItem.Quantity.ToString();
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
        CurrentItem = null;

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
}