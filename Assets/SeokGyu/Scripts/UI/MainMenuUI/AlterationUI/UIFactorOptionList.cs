using EverScord;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UIFactorOptionList : MonoBehaviour
{
    [SerializeField] private UIFactorOption option;
    [SerializeField] private Transform containor;
    [SerializeField] private OptionList[] optionLists;
    private List<UIFactorOption> options = new List<UIFactorOption>();

    private void Awake()
    {
        UIFactor.OnDisplayOptionList += HandleDisplayOptionList;
        UIFactorOption.OnSelectOption += HandleSelectOption;

        Init();
    }

    private void OnDestroy()
    {
        UIFactor.OnDisplayOptionList -= HandleDisplayOptionList;
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
                Initialize(optionLists[i].DataList.OptionDatas.Length, optionLists[i].DataList);
                break;
            }
        }
    }

    private void HandleSelectOption()
    {
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
            UIFactorOption op = Instantiate(option, containor);
            options.Add(op);
        }

        gameObject.SetActive(false);
    }

    private void Initialize(int count, FactorDatas datas)
    {
        for (int i = 0; i < options.Count; i++)
        {
            if(i < count)
            {
                options[i].gameObject.SetActive(true);
                options[i].Initialize(datas.OptionDatas[i].Name, datas.OptionDatas[i].Values[datas.OptionDatas[i].Values.Length - 1]);
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
