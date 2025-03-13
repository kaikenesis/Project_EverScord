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

    private bool bOptionPanel;

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
            OnPressedEscape();
        }
    }

    private void OnPressedEscape()
    {
        if(bOptionPanel)
        {

        }
        else
        {

        }
    }
}
