using System.Collections.Generic;
using Unity.VisualScripting;
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
        [SerializeField] private Sprite titleSourceImg;
        [SerializeField] private Color titleTextColor;
        [SerializeField] private Sprite slotSourceImg;
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

        public Sprite TitleSourceImg
        {
            get { return titleSourceImg; }
            private set { titleSourceImg = value; }
        }

        public Color TitleTextColor
        {
            get { return titleTextColor; }
            private set { titleTextColor = value; }
        }

        public Sprite SlotSourceImg
        {
            get { return slotSourceImg; }
            private set { slotSourceImg = value; }
        }

        public OptionData[] OptionDatas
        {
            get { return optionDatas; }
            private set { optionDatas = value; }
        }

        public void SetRandomOption(out Sprite sprite, out Color color, out string name, out float value, out int index)
        {
            index = Random.Range(0, OptionDatas.Length);

            sprite = OptionDatas[index].SourceImg;
            color = OptionDatas[index].ImgColor;
            name = OptionDatas[index].Name;
            value = GetRandomValueMinMax(index);
        }

        // Min값과 Max값 사이 랜덤
        private float GetRandomValueMinMax(int optionNum)
        {
            float[] values = OptionDatas[optionNum].Values;
            float dist = 0.5f;
            int min = (int)(values[0] / dist);
            int max = (int)(values[values.Length - 1] / dist);
            int randomValue = Random.Range(min, max);

            return randomValue * dist;
        }

        // 정해진 배열 값들 중에서 랜덤
        private float GetRandomValue(int optionNum)
        {
            float[] values = OptionDatas[optionNum].Values;
            int randomNum = Random.Range(0, values.Length);
            return OptionDatas[randomNum].Values[randomNum];
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
