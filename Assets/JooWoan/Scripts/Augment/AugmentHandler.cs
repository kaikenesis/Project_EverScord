using System.Collections.Generic;
using UnityEngine;
using EverScord.UI;

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

        [SerializeField] private CardUI helmetCardUI, vestCardUI, shoesCardUI;
        private AugmentData augmentData = new AugmentData();

        private List<string> helmetAugmentTags = new();
        private List<string> vestAugmentTags = new();
        private List<string> shoesAugmentTags = new();

        private int enhanceCount = 0;

        void Start()
        {
            augmentData.Init();
            InitAugmentDict(AugmentType.Helmet);
            InitAugmentDict(AugmentType.Vest);
            InitAugmentDict(AugmentType.Shoes);
        }

        void InitAugmentDict(AugmentType type)
        {
            IDictionary<string, HelmetAugment> augmentDict = null;
            List<string> augmentTags = null;
            CardUI targetCard = null;

            switch (type)
            {
                case AugmentType.Helmet:
                    // Check dealer or healer
                    //augmentDict = augmentData.DealerHelmetAugmentDict;
                    augmentTags = helmetAugmentTags;
                    targetCard = helmetCardUI;
                    break;

                case AugmentType.Vest:
                    augmentTags = vestAugmentTags;
                    targetCard = vestCardUI;
                    break;

                case AugmentType.Shoes:
                    augmentTags = shoesAugmentTags;
                    targetCard = shoesCardUI;
                    break;

                default:
                    break;
            }

            if (augmentDict == null)
            {
                Debug.LogWarning($"Failed to initialize augment dictionary.");
                return;
            }

            int index = 0;

            foreach (KeyValuePair<string, HelmetAugment> keyValue in augmentDict)
            {
                if (index >= targetCard.slotImages.Length)
                    break;

                augmentTags.Add(keyValue.Key);
                targetCard.SetSlotText(index);
                index++;
            }
        }
    }
}
