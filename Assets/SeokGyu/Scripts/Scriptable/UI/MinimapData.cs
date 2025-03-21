using UnityEngine;

namespace EverScord
{
    [SerializeField]
    [CreateAssetMenu(menuName = "EverScord/Datas/MinimapData", fileName = "newMinimapData")]
    public class MinimapData : ScriptableObject
    {
        [field: SerializeField] public CameraTransform[] CameraPos { get; private set; }
        [field: SerializeField] public StageMap[] StageMaps { get; private set; }

        [System.Serializable]
        public class CameraTransform
        {
            [field: SerializeField] public Vector3 Position { get; private set; }
            [field: SerializeField] public Quaternion Rotation { get; private set; }
            [field: SerializeField] public float OrthographicSize { get; private set; }
        }

        [System.Serializable]
        public class StageMap
        {
            [field: SerializeField] public Sprite SourceImg { get; private set; }
            [field: SerializeField] public Vector3 Position { get; private set; }
            [field: SerializeField] public Vector2 Size { get; private set; }
            [field: SerializeField] public Quaternion Rotation { get; private set; }
        }
    }
}

