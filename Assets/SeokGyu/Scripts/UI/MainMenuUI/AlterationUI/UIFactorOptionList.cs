using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EverScord
{
    public class UIFactorOptionList : MonoBehaviour
    {
        [SerializeField] private GameObject option;
        [SerializeField] private Transform containor;
        [SerializeField] private OptionList[] optionLists;
        private List<UIFactorOption> options = new List<UIFactorOption>();

        public static Action<Color> OnRequestApplyOption = delegate { };
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

            int count = (int)UIFactorSlot.EType.MAX;
            for (int i = 0; i < count; i++)
            {
                if ((int)optionLists[i].Type == typeNum)
                {
                    FactorDatas datas = optionLists[i].DataList;
                    DisplayOption(datas.OptionDatas.Length, datas, typeNum);
                    break;
                }
            }
        }

        private void HandleSelectOption(int typeNum, int optionNum)
        {
            FactorDatas factorDatas = optionLists[typeNum].DataList;
            Color optionImgColor = factorDatas.OptionDatas[optionNum].ImgColor;
            OnRequestApplyOption?.Invoke(optionImgColor);

            gameObject.SetActive(false);
        }

        private void Init()
        {
            int max = -1;
            for (int i = 0; i < optionLists.Length; i++)
            {
                if (max < optionLists[i].DataList.OptionDatas.Length)
                    max = optionLists[i].DataList.OptionDatas.Length;
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
            for (int i = 0; i < optionLists.Length; i++)
            {
                FactorDatas.OptionData[] optionDatas = optionLists[i].DataList.OptionDatas;
                for (int j = 0; j < optionDatas.Length; j++)
                {
                    OnInitializeOptionName?.Invoke(optionDatas[j].Name);
                }
            }

            gameObject.SetActive(false);
        }

        private void DisplayOption(int count, FactorDatas datas, int typeNum)
        {
            for (int i = 0; i < options.Count; i++)
            {
                if (i < count)
                {
                    options[i].gameObject.SetActive(true);
                    FactorDatas.OptionData optionData = datas.OptionDatas[i];
                    options[i].Initialize(optionData.Name, optionData.Values[optionData.Values.Length - 1], i, typeNum);
                    //OnInitializeOptionName?.Invoke(optionData.Name);
                }
                else
                    options[i].gameObject.SetActive(false);
            }
        }

        [System.Serializable]
        public class OptionList
        {
            [SerializeField] private UIFactorSlot.EType type;
            [SerializeField] private FactorDatas dataList;

            public UIFactorSlot.EType Type
            {
                get { return type; }
                private set { type = value; }
            }

            public FactorDatas DataList
            {
                get { return dataList; }
                private set { dataList = value; }
            }
        }
    }
}
