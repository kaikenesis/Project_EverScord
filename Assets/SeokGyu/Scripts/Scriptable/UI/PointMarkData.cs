using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace EverScord
{
    [SerializeField]
    [CreateAssetMenu(menuName = "EverScord/Datas/PointMarkData", fileName = "newPointMarkData")]
    public class PointMarkData : ScriptableObject
    {
        public enum EType
        {
            Player,
            Monster = 3,
            BossMonster,
            Portal,
        }

        [SerializeField] private Marker[] markers;

        public void SetMarker(int typeNum, Image image, out Vector2 size)
        {
            image.sprite = markers[typeNum].ResourceImg;
            image.color = markers[typeNum].Color;
            size = markers[typeNum].Size;
        }

        [System.Serializable]
        public class Marker
        {
            [SerializeField] private Sprite resourceImg;
            [SerializeField] private Color color;
            [SerializeField] private Vector2 size;

            public Sprite ResourceImg
            {
                get { return resourceImg; }
            }

            public Color Color
            {
                get { return color; }
            }

            public Vector2 Size
            {
                get { return size; }
            }
        }
    }
}
