using TMPro;
using UnityEngine;

namespace EverScord
{
    public class UIProbabilitySheetPresenter : MonoBehaviour
    {
        [SerializeField] private FactorModel model;
        [SerializeField] private GameObject sheetOption;

        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            int typeCount = model.Datas.Length;
            for (int i = 0; i < typeCount; i++)
            {
                GameObject obj = Instantiate(sheetOption, transform);
                UISheetOptionPresenter uiSheetOption = obj.GetComponent<UISheetOptionPresenter>();
                uiSheetOption.SetOptionInfo(i);
            }
        }
    }
}

