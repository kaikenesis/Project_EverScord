using UnityEngine;

namespace EverScord
{
    [SerializeField]
    [CreateAssetMenu(menuName = "EverScord/Datas/FactorData", fileName = "newFactorData")]
    public class FactorData : ScriptableObject
    {
        public enum EType
        {
            ALPHA,
            BETA
        }

        [SerializeField] private EType type;
        [SerializeField] private int slotCount;
        [SerializeField] private int confirmedCount;
        [SerializeField] private Sprite lockedSourceImg;
        [SerializeField] private OptionData[] optionDatas;

        public EType Type
        {
            get { return type; }
        }

        public int SlotCount
        {
            get { return slotCount; }
        }

        public int ConfirmedCount
        {
            get { return confirmedCount; }
        }

        public Sprite LockedSourceImg
        {
            get { return lockedSourceImg; }
            private set { lockedSourceImg = value; }
        }

        public OptionData[] OptionDatas
        {
            get { return optionDatas; }
            private set { optionDatas = value; }
        }

        [System.Serializable]
        public class OptionData
        {
            [SerializeField] private string _name;
            [SerializeField] private float[] values;
            [SerializeField] private Sprite sourceImg;
            [SerializeField] private Color imgColor;

            public string Name
            {
                get { return _name; }
                private set { _name = value; }
            }

            public float[] Values
            {
                get { return values; }
                private set { values = value; }
            }

            public Sprite SourceImg
            {
                get { return sourceImg; }
                private set { sourceImg = value; }
            }

            public Color ImgColor
            {
                get { return imgColor; }
                private set { imgColor = value; }
            }
        }
    }
}
