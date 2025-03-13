using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EType = EverScord.ArmorData.Armor.EType;

namespace EverScord
{
    public class UIArmor : MonoBehaviour
    {
        [SerializeField] private GameObject armorSlotPrefab;
        [SerializeField] private EType[] armorType;
        private List<Image> armors = new();

        private void Awake()
        {
            Initialize();
        }

        private void Start()
        {
            GameManager.Instance.AugmentControl.SubscribeOnEnhanced(GameManager.Instance.ArmorData.LevelUpArmors);
            GameManager.Instance.ArmorData.SubscribeOnLevelUp(UpdateArmorUI);
        }

        private void Initialize()
        {
            for (int i = 0; i < armorType.Length; i++)
            {
                GameObject obj = Instantiate(armorSlotPrefab, transform);
                int index = GameManager.Instance.ArmorData.Armors[i].CurLevel;
                Image image = obj.GetComponent<Image>();
                image.sprite = GameManager.Instance.ArmorData.Armors[i].SourceImg[index - 1];
                armors.Add(image);
            }
        }

        private void HandleArmorUpdated()
        {
            Initialize();
        }

        private void UpdateArmorUI()
        {
            for (int i = 0; i < armorType.Length; i++)
            {
                int level = GameManager.Instance.ArmorData.Armors[i].CurLevel;
                armors[i].sprite = GameManager.Instance.ArmorData.Armors[i].SourceImg[level - 1];
            }
        }
    }
}
