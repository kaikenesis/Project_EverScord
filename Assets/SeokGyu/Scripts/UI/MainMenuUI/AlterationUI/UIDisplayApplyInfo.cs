using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace EverScord
{
    public class UIDisplayApplyInfo : MonoBehaviour
    {
        [SerializeField] private TMP_Text infoText;

        private List<OptionInfo> options = new List<OptionInfo>();

        private void Awake()
        {
            UIFactorOptionList.OnInitializeOptionName += HandleInitializeOptionName;
            UIFactorSlot.OnRequestUpdateInfo += HandleRequestUpdateInfo;
        }

        private void OnDestroy()
        {
            UIFactorOptionList.OnInitializeOptionName -= HandleInitializeOptionName;
            UIFactorSlot.OnRequestUpdateInfo -= HandleRequestUpdateInfo;
        }

        private void Start()
        {
            LoadInfo();
        }

        private void OnEnable()
        {
            UpdateInfo();
        }

        private void HandleInitializeOptionName(string name)
        {
            OptionInfo optionInfo = new OptionInfo(name, 0.0f);
            options.Add(optionInfo);
        }

        private void HandleRequestUpdateInfo(string newName, float newValue, string prevName, float prevValue)
        {
            for (int i = 0; i < options.Count; i++)
            {
                if (options[i].name == prevName)
                {
                    options[i].value -= prevValue;
                }

                if (options[i].name == newName)
                {
                    options[i].value += newValue;
                }
            }

            Debug.Log($"prevName : {prevName}, prevValue : {prevValue}, newName : {newName}, newValue : {newValue}");

            UpdateInfo();
        }

        private void LoadInfo()
        {
            List<AlterationData.PanelData> panelDatas = GameManager.Instance.PlayerAlterationData.PanelDatas;

            for (int i = 0; i < panelDatas.Count; i++)
            {
                if (panelDatas[i].OptionNum.Length <= 0) return;
                    
                for (int j = 0; j < panelDatas[i].OptionNum.Length; j++)
                {
                    if (panelDatas[i].OptionNum[j] == -1) continue;

                    FactorData.OptionData optionData = GameManager.Instance.FactorDatas[i].OptionDatas[panelDatas[i].OptionNum[j]];
                    HandleRequestUpdateInfo(optionData.Name, panelDatas[i].Value[j], "", 0f);
                }
            }
        }

        private void UpdateInfo()
        {
            infoText.text = "";
            float[] values = new float[options.Count];
            for (int i = 0; i < options.Count; i++)
            {
                values[i] = options[i].value;
                if (options[i].value == 0.0f) continue;
                infoText.text += $"{options[i].name} : {options[i].value}\n";
            }

            
        }

        public class OptionInfo
        {
            public string name { get; private set; }
            public float value;

            public OptionInfo(string name, float value)
            {
                this.name = name;
                this.value = value;
            }
        }
    }
}
