using UnityEngine;
using EverScord.Character;

namespace EverScord.UI
{
    public class CustomCursor
    {
        //private Transform aimPoint;
        //private Camera mainCam;

        //public CustomCursor(CharacterControl player)
        //{
        //    mainCam  = player.CameraControl.Cam;
        //    aimPoint = player.PlayerWeapon.AimPoint;
        //}

        //// 0,0 for the canvas is at the center of the screen
        //// WorldToViewPortPoint treats the lower left corner as 0,0
        //public void SetCursorPosition(PlayerUI playerUI)
        //{
        //    Vector2 viewPortPosition = mainCam.WorldToViewportPoint(aimPoint.position);

        //    Vector2 screenPosition = new Vector2(
        //        (viewPortPosition.x - 0.5f) * playerUI.CanvasRect.sizeDelta.x,
        //        (viewPortPosition.y - 0.5f) * playerUI.CanvasRect.sizeDelta.y
        //    );

        //    playerUI.CursorRect.anchoredPosition = screenPosition;
        //}
    }
}

