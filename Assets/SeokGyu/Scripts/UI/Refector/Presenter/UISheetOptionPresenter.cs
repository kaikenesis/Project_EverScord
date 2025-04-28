using System.Threading;
using TMPro;
using UnityEngine;

namespace EverScord
{
    public class UISheetOptionPresenter : MonoBehaviour
    {
        [SerializeField] private FactorModel model;
        [SerializeField] private UISheetOptionView view;
        
        public void SetOptionInfo(int num)
        {
            string typeText = $"��{model.Datas[num].TypeName}";
            view.SetTypeText(typeText);

            FactorModel.OptionData[] optionDatas = model.Datas[num].OptionDatas;
            int count = optionDatas.Length;

            for (int i = 0; i < count; i++)
            {
                GameObject obj = new GameObject("Option");

                string text = $"�� <color=white>{optionDatas[i].Name}</color>({optionDatas[i].Values[0]}%~{optionDatas[i].Values[optionDatas[i].Values.Length - 1]}%)";
                view.AddTextToContainor(obj, text);
            }

            view.SetHeight(count);
        }
    }
}
