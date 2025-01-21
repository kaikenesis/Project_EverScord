using UnityEngine;

namespace EverScord.UI
{
    public class CustomCursor : MonoBehaviour
    {
        [SerializeField] private Transform aimPoint;
        [SerializeField] private RectTransform canvasRect;

        private RectTransform uiRect;
        private Camera mainCam;

        void Start()
        {
            uiRect = GetComponent<RectTransform>();
            mainCam = Camera.main;
        }

        void Update()
        {
            SetCursorPosition();
        }

        // WorldToViewPortPoint treats the lower left corner as 0,0
        private void SetCursorPosition()
        {
            Vector2 viewPortPosition = mainCam.WorldToViewportPoint(aimPoint.position);
            Vector2 screenPosition = new Vector2(
                (viewPortPosition.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f),
                (viewPortPosition.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f)
            );

            uiRect.anchoredPosition = screenPosition;
        }
    }
}

