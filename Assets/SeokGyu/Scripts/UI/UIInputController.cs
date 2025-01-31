using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIInputController : MonoBehaviour
{
    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject partyOption;
    private GraphicRaycaster m_Raycaster;
    private PointerEventData m_PointerEventData;
    private EventSystem m_EventSystem;

    private void Awake()
    {
        m_Raycaster = canvas.GetComponent<GraphicRaycaster>();
        m_EventSystem = GetComponent<EventSystem>();
    }

    private void Update()
    {
        OnMouseClick();
    }

    private void OnMouseClick()
    {
        if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
        {
            m_PointerEventData = new PointerEventData(m_EventSystem);
            m_PointerEventData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();

            m_Raycaster.Raycast(m_PointerEventData, results);

            if (results.Count <= 0)
            {
                // 델리게이트 사용?
                partyOption.SetActive(false);
                return;
            }

            for (int i = 0;i< results.Count;i++)
            {
                if (results[i].gameObject.tag == "UIPlayer")
                    return;
            }

            partyOption.SetActive(false);
        }
    }
}
