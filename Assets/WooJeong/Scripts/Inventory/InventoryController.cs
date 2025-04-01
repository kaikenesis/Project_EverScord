using DG.Tweening;
using EverScord;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [SerializeField] private Item item;
    [SerializeField] private Item item2;

    [SerializeField] private GameObject inventoryPanel;    

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            InventoryManager.Instance.AddItem(item);
        if (Input.GetKeyDown(KeyCode.S))
            InventoryManager.Instance.AddItem(item2);
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
}
