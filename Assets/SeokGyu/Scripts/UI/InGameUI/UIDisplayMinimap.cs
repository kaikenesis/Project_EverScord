using UnityEngine;

namespace EverScord
{
    public class UIDisplayMinimap : MonoBehaviour
    {
        [SerializeField] private GameObject minimapUI;
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

            Instantiate(minimapUI);
            minimapUI.GetComponent<RectTransform>().localRotation = Quaternion.Euler(new Vector3(-90, 0, 0));
        }
    }
}

