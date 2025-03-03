using UnityEngine;
using UnityEngine.UI;

namespace EverScord
{
    public class UIArmor : MonoBehaviour
    {
        [SerializeField] private GameObject armorSlotPrefab;
        [SerializeField] private ArmorData.Armor.EType[] armorType;

        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            for (int i = 0; i < armorType.Length; i++)
            {
                GameObject obj = Instantiate(armorSlotPrefab, transform);
                int index = GameManager.Instance.ArmorData.Armors[i].CurLevel;
                obj.GetComponent<Image>().sprite = GameManager.Instance.ArmorData.Armors[i].SourceImg[index - 1];
            }
        }

        private void HandleArmorUpdated()
        {
            Initialize();
        }
    }
}
