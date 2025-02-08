using TMPro;
using UnityEngine;

namespace EverScord
{
    public class UIFactorPanel : MonoBehaviour
    {
        [SerializeField] private GameObject factor;
        [SerializeField] private Transform containor;
        [SerializeField] private TMP_Text text;

        public void Initialize(UIFactorSlot.EType type, int slotCount, int confirmedCount)
        {
            SetTitle(type);

            for (int i = 0; i < slotCount; i++)
            {
                bool bConfirmed = false;
                if (i < confirmedCount)
                    bConfirmed = true;

                GameObject obj = Instantiate(factor, containor);
                obj.SetActive(true);

                UIFactorSlot slot = obj.GetComponent<UIFactorSlot>();
                slot.Initialize(type, bConfirmed, i);
            }
        }

        private void SetTitle(UIFactorSlot.EType type)
        {
            switch (type)
            {
                case UIFactorSlot.EType.ALPHA:
                    text.text = "Alpha";
                    break;
                case UIFactorSlot.EType.BETA:
                    text.text = "Beta";
                    break;
            }
        }
    }
}
