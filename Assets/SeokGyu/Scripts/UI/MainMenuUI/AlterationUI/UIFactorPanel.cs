using Photon.Pun.Demo.Procedural;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static EverScord.UISkill;

namespace EverScord
{
    public class UIFactorPanel : MonoBehaviour
    {
        [SerializeField] private GameObject factor;
        [SerializeField] private Transform containor;
        [SerializeField] private TMP_Text text;

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
            List<UIFactorSlot> slots = GameManager.Instance.PlayerAlterationData[type].slots;
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

            List<UIFactorSlot> slots = GameManager.Instance.PlayerAlterationData[typeNum].slots;
            for (int i = 0; i < slotCount; i++)
            {
                bool bConfirmed = false;
                if (i < confirmedCount)
                    bConfirmed = true;

                GameObject obj = Instantiate(factor, containor);
                obj.SetActive(true);

                UIFactorSlot slot = obj.GetComponent<UIFactorSlot>();
                slot.Initialize(typeNum, bConfirmed, i);

                if (slots.Count <= i)
                    slots.Add(slot);
            }
        }

        private void SetTitle(int typeNum)
        {
            panelTypeNum = typeNum;

            switch (typeNum)
            {
                case 0:
                    text.text = "Alpha";
                    break;
                case 1:
                    text.text = "Beta";
                    break;
            }
        }
    }
}
