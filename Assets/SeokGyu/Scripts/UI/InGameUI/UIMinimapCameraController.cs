using UnityEngine;

namespace EverScord
{
    public class UIMinimapCameraController : MonoBehaviour
    {
        [SerializeField] private GameObject minimapCamera;
        [SerializeField] private GameObject portalIcon;
        [SerializeField] private Transform[] cameraPos;
        [SerializeField] private Transform[] portalIconPos;

        private void Awake()
        {
            minimapCamera.transform.SetParent(cameraPos[0]);
            minimapCamera.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            portalIcon.transform.SetParent(portalIconPos[0]);
            portalIcon.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }
    }
}
