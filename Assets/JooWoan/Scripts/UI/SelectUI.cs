using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Events;
using EverScord.Augment;

using ArmorType = EverScord.ArmorData.Armor.EType;

namespace EverScord.UI
{
    public class SelectUI : MonoBehaviour
    {
        [SerializeField] private Transform slotParent;
        [SerializeField] private Color selectedSlotColor;
        [SerializeField] private Image iconImage;
        [SerializeField] private ArmorType armorType;

        private Color32 initialColor;
        private List<TextMeshProUGUI> slotTexts = new();

        public Image[] slotImages { get; private set; }
        public int selectedSlotIndex { get; private set; }

        private UnityEvent onSelectSlotEvent = new UnityEvent();

        void Awake()
        {
            slotImages = slotParent.GetComponentsInChildren<Image>(true);
            
            if (slotImages.Length > 0)
                initialColor = slotImages[0].color;

            iconImage.sprite = AugmentPresenter.GetArmorIcon(armorType, 1);
        }

        public void Init(UnityAction listener)
        {
            onSelectSlotEvent?.AddListener(listener);
            
            foreach (Image slot in slotImages)
                slotTexts.Add(slot.GetComponentInChildren<TextMeshProUGUI>(true));

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
            SoundManager.Instance.PlaySound("ButtonSound");
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

        public void SetSlotText(int index, ArmorAugment augment)
        {
            if (augment == null)
            {
                Debug.LogWarning("Null armor augment detected.");
                return;
            }

            float value = augment.DescriptionValue;
            if (value < 0)
                value *= -1;
            
            slotTexts[index].text = augment.Description.Replace("@", $"{value:G}%");
        }

        public string GetSlotText(int index)
        {
            if (index >= slotTexts.Count)
            {
                Debug.LogWarning("Wrong augment select slot index");
                return "?";
            }
            
            return slotTexts[index].text;
        }

        private void SetSlotColor(int index, bool isSelected)
        {
            if (index == -1 || slotImages.Length == 0)
                return;

            slotImages[index].color = isSelected ? selectedSlotColor : initialColor;
        }
    }
}

