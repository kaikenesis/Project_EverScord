using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace EverScord
{
    public class UIFactorOptionList : MonoBehaviour
    {
        [SerializeField] private GameObject option;
        [SerializeField] private Transform containor;
        private List<UIFactorOption> options = new List<UIFactorOption>();

        public static Action<Color, string, float> OnApplyOption = delegate { };
        public static Action<string> OnInitializeOptionName = delegate { };

        private void Awake()
        {
            UIFactorSlot.OnDisplayOptionList += HandleDisplayOptionList;
            UIFactorOption.OnSelectOption += HandleSelectOption;

            Init();
        }

        private void OnDestroy()
        {
            UIFactorSlot.OnDisplayOptionList -= HandleDisplayOptionList;
            UIFactorOption.OnSelectOption -= HandleSelectOption;
        }

        private void HandleDisplayOptionList(int typeNum)
        {
            gameObject.SetActive(true);

            FactorData datas = GameManager.Instance.FactorDatas[typeNum];
            DisplayOption(datas.OptionDatas.Length, datas, typeNum);
        }

        private void HandleSelectOption(int typeNum, int optionNum, float value)
        {
            FactorData datas = GameManager.Instance.FactorDatas[typeNum];
            Color optionImgColor = datas.OptionDatas[optionNum].ImgColor;
            string optionName = datas.OptionDatas[optionNum].Name;

            OnApplyOption?.Invoke(optionImgColor, optionName, value);

            gameObject.SetActive(false);
        }

        private void Init()
        {
            int max = -1;
            int count = GameManager.Instance.FactorDatas.Length;
            for (int i = 0; i < count; i++)
            {
                FactorData datas = GameManager.Instance.FactorDatas[i];

                if (max < datas.OptionDatas.Length)
                    max = datas.OptionDatas.Length;
            }

            for (int i = 0; i < max; i++)
            {
                GameObject obj = Instantiate(option, containor);
                UIFactorOption factorOption = obj.GetComponent<UIFactorOption>();
                options.Add(factorOption);
            }
        }

        private void Start()
        {
            int count = GameManager.Instance.FactorDatas.Length;
            for (int i = 0; i < count; i++)
            {
                FactorData.OptionData[] optionDatas = GameManager.Instance.FactorDatas[i].OptionDatas;
                for (int j = 0; j < optionDatas.Length; j++)
                {
                    OnInitializeOptionName?.Invoke(optionDatas[j].Name);
                }
            }

            gameObject.SetActive(false);
        }

        private void DisplayOption(int count, FactorData datas, int typeNum)
        {
            for (int i = 0; i < options.Count; i++)
            {
                if (i < count)
                {
                    options[i].gameObject.SetActive(true);
                    FactorData.OptionData optionData = datas.OptionDatas[i];
                    options[i].Initialize(optionData.Name, optionData.Values[optionData.Values.Length - 1], i, typeNum);
                }
                else
                    options[i].gameObject.SetActive(false);
            }
        }
    }
}
