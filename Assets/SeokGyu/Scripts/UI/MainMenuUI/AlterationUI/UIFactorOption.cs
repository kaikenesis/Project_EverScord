using System;
using TMPro;
using UnityEngine;

namespace EverScord
{
    public class UIFactorOption : MonoBehaviour
    {
        [SerializeField] private TMP_Text nameText;
        private float value;
        private int optionNum;
        private int typeNum;

        public static Action<int,int> OnSelectOption = delegate { };

        public void Initialize(string name, float value, int optionNum, int typeNum)
        {
            nameText.text = name;
            this.value = value;
            this.optionNum = optionNum;
            this.typeNum = typeNum;
        }

        public void OnClicked()
        {
            // ������ �ɼ� �ִ��ġ ����
            Debug.Log($"MaxValue : {value}");
            OnSelectOption?.Invoke(typeNum, optionNum);
        }
    }
}
