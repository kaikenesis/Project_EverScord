using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Events;

namespace EverScord.UI
{
    public class CardUI : MonoBehaviour
    {
        [SerializeField] private Transform slotParent;
        [SerializeField] private Color selectedSlotColor;

        private Color32 initialColor;
        private List<TextMeshProUGUI> slotTexts = new();

        public Image[] slotImages { get; private set; }
        public int selectedSlotIndex { get; private set; }

        private UnityEvent onSelectSlotEvent = new UnityEvent();

        void Awake()
        {
            slotImages = slotParent.GetComponentsInChildren<Image>(true);

            foreach (Image slot in slotImages)
                slotTexts.Add(slot.GetComponentInChildren<TextMeshProUGUI>(true));

            selectedSlotIndex = -1;

            if (slotImages.Length > 0)
                initialColor = slotImages[0].color;
        }

        public void OnSelectSlot(int index)
        {
            SetSlotColor(selectedSlotIndex, false);
            selectedSlotIndex = index;
            SetSlotColor(selectedSlotIndex, true);

            onSelectSlotEvent?.Invoke();
        }

        public void SetSlotSelectEvent(UnityAction listener)
        {
            onSelectSlotEvent?.AddListener(listener);
        }

        public void RemoveSlotSelectEvent(UnityAction listener)
        {
            onSelectSlotEvent?.RemoveListener(listener);
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

