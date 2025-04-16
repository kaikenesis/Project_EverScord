using TMPro;
using UnityEngine;

namespace EverScord
{
    public class UISheetOptionView : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        [SerializeField] private GameObject containor;

        public void Initialize(int typeNum)
        {
            FactorData.OptionData[] datas = GameManager.Instance.FactorDatas[typeNum].OptionDatas;

            switch (GameManager.Instance.FactorDatas[typeNum].Type)
            {
                case FactorData.EType.ALPHA:
                    text.text = "▷알파";
                    break;
                case FactorData.EType.BETA:
                    text.text = "▷베타";
                    break;
            }

            int count = datas.Length;
            for (int i = 0; i < count; i++)
            {
                TMP_Text newOption = Instantiate(text, containor.transform);
                newOption.text = $"● <color=white>{datas[i].Name}</color>({datas[i].Values[0]}%~{datas[i].Values[datas[i].Values.Length - 1]}%)";
            }

            float h = text.GetComponent<RectTransform>().sizeDelta.y;

            float height = h * 3 + count / 2 * h;
            GetComponent<RectTransform>().sizeDelta = new Vector2(0, height);
        }
    }
}
