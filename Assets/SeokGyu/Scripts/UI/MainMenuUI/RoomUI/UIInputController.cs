using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIInputController : MonoBehaviour
{
    [SerializeField] private GameObject canvas;
    private GraphicRaycaster m_Raycaster;
    private PointerEventData m_PointerEventData;
    private EventSystem m_EventSystem;

    public static Action<bool, Vector2> OnClickedPlayerUI = delegate { };

    private void Awake()
    {
        m_Raycaster = canvas.GetComponent<GraphicRaycaster>();
        m_EventSystem = GetComponent<EventSystem>();
    }

    private void Update()
    {
        OnMouseClick();
        OnKeyInput();
    }

    private void OnMouseClick()
    {
        if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
        {
            m_PointerEventData = new PointerEventData(m_EventSystem);
            m_PointerEventData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();

            m_Raycaster.Raycast(m_PointerEventData, results);

            for (int i = 0; i < results.Count; i++)
            {
                if (results[i].gameObject.tag == "UIPlayer")
                    return;
            }

            OnClickedPlayerUI?.Invoke(false, m_PointerEventData.position);
        }
    }

    private void OnKeyInput()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            // ESC 입력을 받았을때 이벤트 처리, 하나씩? 한꺼번에? 한꺼번에 처리하는게 쉽긴하다..
            // 우선 키입력은 보류
        }
    }
}
