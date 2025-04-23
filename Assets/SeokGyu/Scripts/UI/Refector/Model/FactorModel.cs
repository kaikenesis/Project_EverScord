using UnityEngine;

namespace EverScord
{
    [SerializeField]
    [CreateAssetMenu(menuName = "EverScord/Datas/FactorDatas", fileName = "newFactorDatas")]
    public class FactorModel : ScriptableObject
    {
        [field: SerializeField] public FactorData[] Datas { get; private set; }

        [System.Serializable]
        public class FactorData
        {
            public enum EType
            {
                ALPHA,
                BETA
            }

            [field: SerializeField] public string TypeName { get; private set; }
            [field: SerializeField] public EType Type { get; private set; }
            [field: SerializeField] public int SlotCount { get; private set; }
            [field: SerializeField] public int ConfirmedCount { get; private set; }
            [field: SerializeField] public Sprite LockedSourceImg { get; private set; }
            [field: SerializeField] public Sprite TitleSourceImg { get; private set; }
            [field: SerializeField] public Color TitleTextColor { get; private set; }
            [field: SerializeField] public Sprite SlotSourceImg { get; private set; }
            [field: SerializeField] public OptionData[] OptionDatas { get; private set; }

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
        }

        [System.Serializable]
        public class OptionData
        {
            [field:SerializeField] public string Name { get; private set; }
            [field:SerializeField] public float[] Values { get; private set; }
            [field:SerializeField] public Sprite SourceImg { get; private set; }
            [field:SerializeField] public Color ImgColor { get; private set; }
        }
    }
}
