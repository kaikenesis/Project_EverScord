using UnityEngine;

namespace EverScord
{
    [SerializeField]
    [CreateAssetMenu(menuName = "EverScord/Game/FactorDatas", fileName = "newFactorDatas")]
    public class FactorDatas : ScriptableObject
    {
        [SerializeField] private OptionData[] optionDatas;

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

            public Color ImgColor
            {
                get { return imgColor; }
                private set { imgColor = value; }
            }
        }
    }
}
