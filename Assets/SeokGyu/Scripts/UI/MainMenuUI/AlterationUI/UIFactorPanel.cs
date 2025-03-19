using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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
        private Sequence popupTween;

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

        private void OnEnable()
        {
            PlayPopupTween();
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

        public void Initialize(int typeNum, int slotCount, int confirmedCount, float tweenDelay)
        {
            SetTitle(typeNum);

            popupTween = CreatePopupTween(GetComponent<CanvasGroup>(), tweenDelay);
            PlayPopupTween();

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

        private Sequence CreatePopupTween(CanvasGroup canvasGroup, float delay)
        {
            if (canvasGroup == null)
                return null;
            
            Sequence sequence = DOTween.Sequence()
                .SetAutoKill(false)
                .SetUpdate(true)
                .Append(canvasGroup.DOFade(1.0f, 1.5f).From(0f))
                .Join(canvasGroup.transform.DOLocalMoveY(0f, 1.5f).From(-150f))
                .SetEase(Ease.OutBack)
                .SetDelay(delay);

            return sequence;
        }

        private void PlayPopupTween()
        {
            if (popupTween == null)
                return;
            
            popupTween.Goto(0, true);
        }
    }
}
