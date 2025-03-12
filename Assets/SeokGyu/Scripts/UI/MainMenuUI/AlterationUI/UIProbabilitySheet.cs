using TMPro;
using UnityEngine;

namespace EverScord
{
    public class UIProbabilitySheet : MonoBehaviour
    {
        [SerializeField] private GameObject option;

        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
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
