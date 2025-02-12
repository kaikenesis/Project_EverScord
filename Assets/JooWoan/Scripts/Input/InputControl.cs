using UnityEngine;

namespace EverScord.Character
{
    public static class InputControl
    {
        public static InputInfo ReceiveInput()
        {
            InputInfo info = new InputInfo();

            info.horizontalInput         = Input.GetAxisRaw(ConstStrings.INPUT_HORIZONTAL);
            info.verticalInput           = Input.GetAxisRaw(ConstStrings.INPUT_VERTICAL);
            info.mouseAxisX              = Input.GetAxis(ConstStrings.INPUT_MOUSE_X);
            info.holdLeftMouseButton     = Input.GetMouseButton(0);
            info.pressedLeftMouseButton  = Input.GetMouseButtonDown(0);
            info.releasedLeftMouseButton = Input.GetMouseButtonUp(0);
            info.pressedReloadButton     = Input.GetKeyDown(KeyCode.E);
            info.pressedFirstSkill       = Input.GetKeyDown(KeyCode.Q);
            info.pressedSecondSkill      = Input.GetKeyDown(KeyCode.R);
            info.mousePosition           = Input.mousePosition;

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
        public float mouseAxisX;
        public float horizontalInput;
        public float verticalInput;
        public bool holdLeftMouseButton;
        public bool pressedLeftMouseButton;
        public bool releasedLeftMouseButton;
        public bool pressedReloadButton;
        public bool pressedFirstSkill;
        public bool pressedSecondSkill;

        public bool PressedSkill(int index)
        {
            switch (index)
            {
                case 0: return pressedFirstSkill;
                case 1: return pressedSecondSkill;
                default: return false;
            }
        }
    }
}

