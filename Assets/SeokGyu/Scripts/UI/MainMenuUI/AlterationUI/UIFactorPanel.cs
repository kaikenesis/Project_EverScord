using Photon.Pun.Demo.Procedural;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace EverScord
{
    public class UIFactorPanel : MonoBehaviour
    {
        [SerializeField] private GameObject factor;
        [SerializeField] private Transform containor;
        [SerializeField] private TMP_Text text;

        private UIFactorSlot.EType panelType;
        private List<UIFactorSlot> slots = new List<UIFactorSlot>();

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
            if ((int)panelType != type || slots[slotNum].bConfirmed == true || slots[slotNum - 1].bLock == true) return;

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

        public void Initialize(UIFactorSlot.EType type, int slotCount, int confirmedCount)
        {
            SetTitle(type);

            for (int i = 0; i < slotCount; i++)
            {
                bool bConfirmed = false;
                if (i < confirmedCount)
                    bConfirmed = true;

                GameObject obj = Instantiate(factor, containor);
                obj.SetActive(true);

                UIFactorSlot slot = obj.GetComponent<UIFactorSlot>();
                slot.Initialize(type, bConfirmed, i);
                slots.Add(slot);
            }
        }

        private void SetTitle(UIFactorSlot.EType type)
        {
            panelType = type;

            switch (type)
            {
                case UIFactorSlot.EType.ALPHA:
                    text.text = "Alpha";
                    break;
                case UIFactorSlot.EType.BETA:
                    text.text = "Beta";
                    break;
            }
        }
    }
}
