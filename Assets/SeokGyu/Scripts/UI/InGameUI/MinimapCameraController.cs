using UnityEngine;

namespace EverScord
{
    public class MinimapCameraController : MonoBehaviour
    {
        [SerializeField] private GameObject minimapCamera;

        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            GameObject cameraObj = Instantiate(minimapCamera);
            MinimapData.CameraTransform cameraTransform = GameManager.Instance.MinimapData.CameraPos[0];
            cameraObj.transform.SetLocalPositionAndRotation(cameraTransform.Position, cameraTransform.Rotation);
        }
    }
}

