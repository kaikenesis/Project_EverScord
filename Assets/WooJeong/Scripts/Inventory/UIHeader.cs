using DG.Tweening;
using EverScord;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIHeader : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private Button exitButton;
    [SerializeField] private string exitTweenID;
    private Vector2 offSet;

    private void Awake()
    {
        uiPanel = transform.parent.gameObject;
        exitButton = GetComponentInChildren<Button>();
        exitButton.onClick.AddListener(Exit);
    }

    public void Exit()
    {
        DOTween.Rewind(exitTweenID);
        DOTween.Play(exitTweenID);
    }

    public void OnDrag(PointerEventData eventData)
    {
        uiPanel.transform.position = eventData.position + offSet;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        offSet = new Vector2(uiPanel.transform.position.x - eventData.position.x,
                            uiPanel.transform.position.y - eventData.position.y);
    }
}
