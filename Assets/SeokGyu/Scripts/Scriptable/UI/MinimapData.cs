using UnityEngine;

namespace EverScord
{
    [SerializeField]
    [CreateAssetMenu(menuName = "EverScord/Datas/MinimapData", fileName = "newMinimapData")]
    public class MinimapData : ScriptableObject
    {
        [SerializeField] private CameraTransform[] cameraPos;
        [SerializeField] private StageMap[] stageMaps;

        public CameraTransform[] CameraPos
        {
            get { return cameraPos; }
        }

        public StageMap[] StageMaps
        {
            get { return stageMaps; }
        }

        [System.Serializable]
        public class CameraTransform
        {
            [SerializeField] private Vector3 position;
            [SerializeField] private Quaternion rotation;

            public Vector3 Position
            {
                get { return position; }
            }

            public Quaternion Rotation
            {
                get { return rotation; }
            }
        }

        [System.Serializable]
        public class StageMap
        {
            [SerializeField] private Sprite sourceImg;
            [SerializeField] private Vector3 position;
            [SerializeField] private Vector2 size;
            [SerializeField] private Quaternion rotation;

            public Sprite SourceImg
            {
                get { return sourceImg; }
            }

            public Vector3 Position
            {
                get { return position; }
            }

            public Vector2 Size
            {
                get { return size; }
            }

            public Quaternion Rotation
            {
                get { return rotation; }
            }
        }
    }
}

