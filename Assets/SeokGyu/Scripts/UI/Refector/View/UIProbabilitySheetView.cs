using TMPro;
using UnityEngine;

namespace EverScord
{
    public class UIProbabilitySheetView : ToggleObject
    {
        [SerializeField] private GameObject option;

        protected override void Initialize()
        {
            base.Initialize();

            int typeCount = GameManager.Instance.FactorDatas.Length;
            for (int i = 0; i < typeCount; i++)
            {
                GameObject obj = Instantiate(option, transform);
                UISheetOption uiSheetOption = obj.GetComponent<UISheetOption>();
                uiSheetOption.Initialize(i);
            }
        }
    }
}
