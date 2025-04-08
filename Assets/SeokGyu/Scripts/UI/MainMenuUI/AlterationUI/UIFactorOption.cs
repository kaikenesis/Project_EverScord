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

        public void Initialize(string name, float value, int optionNum, int typeNum, Sprite sprite)
        {
            nameText.text = $"{name} {value}%";
            optionIcon.sprite = sprite;
            this.value = value;
            this.optionNum = optionNum;
            this.typeNum = typeNum;
        }

        public void OnClicked()
        {
            SoundManager.Instance.PlaySound("ButtonSound");

            OnSelectOption?.Invoke(typeNum, optionNum, value);
        }
    }
}
