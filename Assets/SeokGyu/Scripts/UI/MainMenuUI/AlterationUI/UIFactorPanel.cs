using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EverScord
{
    public class UIFactorPanel : MonoBehaviour
    {
        [SerializeField] private GameObject factor;
        [SerializeField] private Transform containor;
        [SerializeField] private Image titleImg;
        [SerializeField] private TMP_Text text;
        private List<UIFactorSlot> slots = new List<UIFactorSlot>();
        private int panelTypeNum;

        public static Action<int> OnUnlockFactor = delegate { };
        public static Action<int> OnRerollFactor = delegate { };

        private void Awake()
        {
            UIFactorSlot.OnClickedSlot += HandleClickedSlot;
        }

        private void OnDestroy()
        {
            UIFactorSlot.OnClickedSlot -= HandleClickedSlot;
        }

        private void HandleClickedSlot(int type, int slotNum)
        {
            if (panelTypeNum != type || slots[slotNum].bConfirmed == true || slots[slotNum - 1].bLock == true) return;

            if (slots[slotNum].bLock == true)
            {
                int cost = GameManager.Instance.CostDatas.SlotCostDatas[slotNum - 1].Unlock;
                OnUnlockFactor?.Invoke(cost);
            }
            else
            {
                int cost = GameManager.Instance.CostDatas.SlotCostDatas[slotNum - 1].Reroll;
                OnRerollFactor?.Invoke(cost);
            }
        }

        public void Initialize(int typeNum, int slotCount, int confirmedCount)
        {
            SetTitle(typeNum);

            for (int i = 0; i < slotCount; i++)
            {
                bool bConfirmed = false;
                if (i < confirmedCount)
                    bConfirmed = true;

                GameObject obj = Instantiate(factor, containor);
                obj.SetActive(true);

                UIFactorSlot slot = obj.GetComponent<UIFactorSlot>();
                slot.Initialize(typeNum, bConfirmed, i);

                slots.Add(slot);
            }
        }

        private void SetTitle(int typeNum)
        {
            titleImg.sprite = GameManager.Instance.FactorDatas[typeNum].TitleSourceImg;
            text.color = GameManager.Instance.FactorDatas[typeNum].TitleTextColor;
            panelTypeNum = typeNum;

            switch (typeNum)
            {
                case 0:
                    text.text = "알파";
                    break;
                case 1:
                    text.text = "베타";
                    break;
            }
        }
    }
}
