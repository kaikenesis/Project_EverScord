using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Events;

namespace EverScord.UI
{
    public class SelectUI : MonoBehaviour
    {
        [SerializeField] private Transform slotParent;
        [SerializeField] private Color selectedSlotColor;

        private Color32 initialColor;
        private List<TextMeshProUGUI> slotTexts = new();

        public Image[] slotImages { get; private set; }
        public int selectedSlotIndex { get; private set; }

        private UnityEvent onSelectSlotEvent = new UnityEvent();

        public void Init(UnityAction listener)
        {
            onSelectSlotEvent?.AddListener(listener);
            
            if (slotImages == null)
            {
                slotImages = slotParent.GetComponentsInChildren<Image>(true);

                foreach (Image slot in slotImages)
                    slotTexts.Add(slot.GetComponentInChildren<TextMeshProUGUI>(true));

                if (slotImages.Length > 0)
                    initialColor = slotImages[0].color;
            }

            ResetState();
        }

        void OnDisable()
        {
            ResetState();
        }

        private void ResetState()
        {
            selectedSlotIndex = -1;

            for (int i = 0; i < slotImages.Length; i++)
                SetSlotColor(i, false);
        }

        public void OnSelectSlot(int index)
        {
            SetSlotColor(selectedSlotIndex, false);
            selectedSlotIndex = index;
            SetSlotColor(selectedSlotIndex, true);

            onSelectSlotEvent?.Invoke();
        }
        
        public void RemoveSlotSelectEvent(UnityAction listener)
        {
            onSelectSlotEvent?.RemoveListener(listener);
        }

        public void SetSlotIndex(int index)
        {
            selectedSlotIndex = index;
        }

        public void SetSlotText(int index, string description)
        {
            slotTexts[index].text = description;
        }

        private void SetSlotColor(int index, bool isSelected)
        {
            if (index == -1 || slotImages.Length == 0)
                return;

            slotImages[index].color = isSelected ? selectedSlotColor : initialColor;
        }
    }
}

