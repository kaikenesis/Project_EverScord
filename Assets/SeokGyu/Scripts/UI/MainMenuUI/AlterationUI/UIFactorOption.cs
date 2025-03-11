using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EverScord
{
    public class UIFactorOption : MonoBehaviour
    {
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private Image optionIcon;
        private float value;
        private int optionNum;
        private int typeNum;

        public static Action<int,int,float> OnSelectOption = delegate { };

        public void Initialize(string name, float value, int optionNum, int typeNum)
        {
            nameText.text = $"{name} {value}%";
            optionIcon.sprite = GameManager.Instance.FactorDatas[typeNum].OptionDatas[optionNum].SourceImg;
            this.value = value;
            this.optionNum = optionNum;
            this.typeNum = typeNum;
        }

        public void OnClicked()
        {
            OnSelectOption?.Invoke(typeNum, optionNum, value);
        }
    }
}
