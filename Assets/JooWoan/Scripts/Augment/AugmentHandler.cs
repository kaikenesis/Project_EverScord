using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EverScord.UI;
using EverScord.Armor;

namespace EverScord.Augment
{
    public class AugmentHandler : MonoBehaviour
    {
        private enum AugmentType
        {
            Helmet,
            Vest,
            Shoes
        };

        [SerializeField] private TestPlayer player;
        [SerializeField] private CardUI helmetCardUI, vestCardUI, shoesCardUI;
        [SerializeField] private LockableButton confirmBtn;
        private AugmentData augmentData = new();

        private List<string> helmetAugmentTags = new();
        private List<string> vestAugmentTags = new();
        private List<string> shoesAugmentTags = new();

        private int enhanceCount = 0;

        void Awake()
        {
            augmentData.Init();

            helmetCardUI.SetSlotSelectEvent(TryUnlockConfirmBtn);
            vestCardUI.SetSlotSelectEvent(TryUnlockConfirmBtn);
            shoesCardUI.SetSlotSelectEvent(TryUnlockConfirmBtn);

            confirmBtn.GetComponent<Button>().onClick.AddListener(EnhanceArmor);
        }

        void OnDisable()
        {
            RemoveSlotSelectEvent();
            confirmBtn.GetComponent<Button>().onClick.RemoveListener(EnhanceArmor);
        }

        void Start()
        {
            SetAugmentTags(AugmentType.Helmet);
            SetAugmentTags(AugmentType.Vest);
            SetAugmentTags(AugmentType.Shoes);
        }

        void SetAugmentTags(AugmentType type)
        {
            helmetAugmentTags.Clear();
            vestAugmentTags.Clear();
            shoesAugmentTags.Clear();

            IDictionary<string, List<ArmorAugment>> augmentDict = null;
            List<string> augmentTags = null;
            CardUI targetCard = null;

            switch (type)
            {
                case AugmentType.Helmet:
                    // Check dealer or healer
                    augmentDict = augmentData.DealerHelmetAugmentDict;

                    augmentTags = helmetAugmentTags;
                    targetCard  = helmetCardUI;
                    break;

                case AugmentType.Vest:
                    augmentDict = augmentData.VestAugmentDict;
                    augmentTags = vestAugmentTags;
                    targetCard  = vestCardUI;
                    break;

                case AugmentType.Shoes:
                    augmentDict = augmentData.ShoesAugmentDict;
                    augmentTags = shoesAugmentTags;
                    targetCard  = shoesCardUI;
                    break;

                default:
                    break;
            }

            if (augmentDict == null)
            {
                Debug.LogWarning("Failed to initialize augment dictionary.");
                return;
            }

            int index = 0;

            foreach (KeyValuePair<string, List<ArmorAugment>> record in augmentDict)
            {
                if (index >= targetCard.slotImages.Length)
                    break;

                if (enhanceCount >= record.Value.Count)
                {
                    Debug.LogWarning($"Enhanced augment does not exist. Current enhance count : {enhanceCount}");
                    break;
                }

                augmentTags.Add(record.Key);
                targetCard.SetSlotText(index, record.Value[enhanceCount]?.Description);
                index++;
            }
        }

        private void TryUnlockConfirmBtn()
        {
            if (helmetCardUI.selectedSlotIndex == -1)
                return;
            
            if (vestCardUI.selectedSlotIndex == -1)
                return;

            if (shoesCardUI.selectedSlotIndex == -1)
                return;

            confirmBtn.UnlockButton();
            RemoveSlotSelectEvent();
        }

        private void EnhanceArmor()
        {
            string helmetTag = helmetAugmentTags[helmetCardUI.selectedSlotIndex];

            // check dealer or healer
            var augmentDict = augmentData.DealerHelmetAugmentDict;
            HelmetAugment helmetAugment = (HelmetAugment)augmentDict[helmetTag][enhanceCount];
            player.SetHelmet(new HelmetDecorator(player.helmet, helmetAugment));

            string vestTag = vestAugmentTags[vestCardUI.selectedSlotIndex];
            VestAugment vestAugment = (VestAugment)augmentData.VestAugmentDict[vestTag][enhanceCount];
            player.SetVest(new VestDecorator(player.vest, vestAugment));
            
            string shoesTag = shoesAugmentTags[shoesCardUI.selectedSlotIndex];
            ShoesAugment shoesAugment = (ShoesAugment)augmentData.ShoesAugmentDict[shoesTag][enhanceCount];
            player.SetShoes(new ShoesDecorator(player.shoes, shoesAugment));

            enhanceCount++;

            if (enhanceCount >= augmentDict[helmetTag].Count)
                enhanceCount = augmentDict[helmetTag].Count - 1;
        }

        private void RemoveSlotSelectEvent()
        {
            helmetCardUI.RemoveSlotSelectEvent(TryUnlockConfirmBtn);
            vestCardUI.RemoveSlotSelectEvent(TryUnlockConfirmBtn);
            shoesCardUI.RemoveSlotSelectEvent(TryUnlockConfirmBtn);
        }
    }
}
