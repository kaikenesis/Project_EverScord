using UnityEngine;

namespace EverScord.Character
{
    public class InputControl
    {
        public static Vector3 ReceiveInput()
        {
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            float verticalInput = Input.GetAxisRaw("Vertical");

            return new Vector3(horizontalInput, 0, verticalInput);
        }

        public static Vector3 ConvertRelativeInput(Vector3 playerInput, Camera playerCam)
        {
            Vector3 camForward = Vector3.Scale(playerCam.transform.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 camRight = Vector3.Scale(playerCam.transform.right, new Vector3(1, 0, 1)).normalized;

            return playerInput.x * camRight + playerInput.z * camForward;
        }
    }
}

