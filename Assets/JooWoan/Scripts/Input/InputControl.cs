using UnityEngine;

namespace EverScord.Character
{
    public static class InputControl
    {
        public static InputInfo ReceiveInput()
        {
            InputInfo info = new InputInfo();

            info.horizontalInput        = Input.GetAxisRaw(ConstStrings.INPUT_HORIZONTAL);
            info.verticalInput          = Input.GetAxisRaw(ConstStrings.INPUT_VERTICAL);
            info.holdLeftMouseButton    = Input.GetMouseButton(0);
            info.mousePosition          = Input.mousePosition;

            return info;
        }

        public static InputInfo GetCameraRelativeInput(InputInfo playerInput, Camera playerCam)
        {
            Vector3 camForward = Vector3.Scale(playerCam.transform.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 camRight = Vector3.Scale(playerCam.transform.right, new Vector3(1, 0, 1)).normalized;

            playerInput.cameraRelativeInput =
                playerInput.horizontalInput * camRight +
                playerInput.verticalInput * camForward;

            return playerInput;
        }
    }

    public struct InputInfo
    {
        public Vector3 cameraRelativeInput;
        public Vector3 mousePosition;
        public float horizontalInput;
        public float verticalInput;
        public bool holdLeftMouseButton;
    }
}

