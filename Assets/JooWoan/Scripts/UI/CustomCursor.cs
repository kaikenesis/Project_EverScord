using UnityEngine;
using EverScord.Weapons;

namespace EverScord.UI
{
    public class CustomCursor : MonoBehaviour
    {
        [SerializeField] private Weapon weapon;
        [SerializeField] private RectTransform canvasRect;

        private Transform aimPoint;
        private RectTransform uiRect;
        private Camera mainCam;

        void Start()
        {
            uiRect = GetComponent<RectTransform>();
            mainCam = Camera.main;
            aimPoint = weapon.AimPoint;
        }

        void Update()
        {
            SetCursorPosition();
        }

        // 0,0 for the canvas is at the center of the screen
        // WorldToViewPortPoint treats the lower left corner as 0,0
        private void SetCursorPosition()
        {
            Vector2 viewPortPosition = mainCam.WorldToViewportPoint(aimPoint.position);

            Vector2 screenPosition = new Vector2(
                ((viewPortPosition.x - 0.5f) * canvasRect.sizeDelta.x),
                ((viewPortPosition.y - 0.5f) * canvasRect.sizeDelta.y)
            );

            uiRect.anchoredPosition = screenPosition;
        }
    }
}

