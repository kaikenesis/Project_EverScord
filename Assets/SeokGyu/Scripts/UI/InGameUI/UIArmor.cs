using UnityEngine;
using UnityEngine.UI;

namespace EverScord
{
    public class UIArmor : MonoBehaviour
    {
        [SerializeField] private GameObject armorSlotPrefab;
        [SerializeField] private ArmorData.Armor.EType[] armorType;
        private Image[] slotImgs;

        private void Awake()
        {
            slotImgs = new Image[armorType.Length];

            for (int i = 0; i < armorType.Length; i++)
            {
                GameObject obj = Instantiate(armorSlotPrefab, transform);
                slotImgs[i] = obj.GetComponent<Image>();
            }
        }

        private void Start()
        {
            for (int i = 0; i < slotImgs.Length; i++)
            {
                int index = GameManager.Instance.ArmorData.Armors[i].CurLevel;
                slotImgs[i].sprite = GameManager.Instance.ArmorData.Armors[i].SourceImg[index - 1];
            }
        }
    }
}
