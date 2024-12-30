using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace EverScord.UI
{
    public class CardUI : MonoBehaviour
    {
        [SerializeField] private Transform slotParent;
        [SerializeField] private Color selectedSlotColor;

        private Color initialColor;
        private List<TextMeshProUGUI> slotTexts = new();

        public Image[] slotImages { get; private set; }
        public int selectedSlotIndex { get; private set; }

        void Awake()
        {
            slotImages = slotParent.GetComponentsInChildren<Image>();

            foreach (Image slot in slotImages)
                slotTexts.Add(slot.GetComponent<TextMeshProUGUI>());

            selectedSlotIndex = -1;

            if (slotImages.Length > 0)
                initialColor = slotImages[0].color;
        }

        public void OnSelectSlot(int index)
        {
            SetSlotColor(selectedSlotIndex, false);
            selectedSlotIndex = index;
            SetSlotColor(selectedSlotIndex, true);
        }

        public void SetSlotText(int index)
        {

        }

        private void SetSlotColor(int index, bool isSelected)
        {
            if (index == -1 || slotImages.Length == 0)
                return;

            slotImages[index].color = isSelected ? selectedSlotColor : initialColor;
        }
    }
}

