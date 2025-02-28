using UnityEngine;

namespace EverScord
{
    [SerializeField]
    [CreateAssetMenu(menuName = "EverScord/Datas/MinimapData", fileName = "newMinimapData")]
    public class MinimapData : ScriptableObject
    {
        [SerializeField] private CameraTransform[] cameraPos;
        [SerializeField] private IconTransform[] portalIconPos;

        [System.Serializable]
        public class CameraTransform
        {
            [SerializeField] private Vector3 position;
            [SerializeField] private Quaternion rotation;
        }

        [System.Serializable]
        public class IconTransform
        {
            [SerializeField] private Vector3 position;
            [SerializeField] private Quaternion rotation;
        }
    }
}

