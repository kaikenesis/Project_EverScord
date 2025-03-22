using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace EverScord
{
    public class UIDisplayApplyInfo : MonoBehaviour
    {
        [SerializeField] private TMP_Text infoText;

        private List<OptionInfo> options = new List<OptionInfo>();

        private void Awake()
        {
            UIFactorSlot.OnRequestUpdateInfo += HandleRequestUpdateInfo;
        }

        private void OnDestroy()
        {
            UIFactorSlot.OnRequestUpdateInfo -= HandleRequestUpdateInfo;
        }

        private void Start()
        {
            LoadInfo();
            UpdateInfoScreen();
        }

        private void OnEnable()
        {
            UpdateInfoScreen();
        }

        private void HandleRequestUpdateInfo(string newName, float newValue, string prevName, float prevValue)
        {
            UpdateInfo(newName, newValue, prevName, prevValue);
            UpdateInfoScreen();
        }

        private void LoadInfo()
        {
            int count = GameManager.Instance.FactorDatas.Length;
            for (int i = 0; i < count; i++)
            {
                FactorData.OptionData[] optionDatas = GameManager.Instance.FactorDatas[i].OptionDatas;
                for (int j = 0; j < optionDatas.Length; j++)
                {
                    OptionInfo optionInfo = new OptionInfo(optionDatas[j].Name, 0.0f);
                    options.Add(optionInfo);
                }
            }

            List<AlterationData.PanelData> panelDatas = GameManager.Instance.PlayerAlterationData.PanelDatas;

            for (int i = 0; i < panelDatas.Count; i++)
            {
                if (panelDatas[i].OptionNum.Length <= 0) return;
                    
                for (int j = 0; j < panelDatas[i].OptionNum.Length; j++)
                {
                    if (panelDatas[i].OptionNum[j] == -1) continue;

                    FactorData.OptionData optionData = GameManager.Instance.FactorDatas[i].OptionDatas[panelDatas[i].OptionNum[j]];
                    UpdateInfo(optionData.Name, panelDatas[i].Value[j], "", 0f);
                }
            }
        }

        private void UpdateInfoScreen()
        {
            infoText.text = "";
            float[] values = new float[options.Count];
            for (int i = 0; i < options.Count; i++)
            {
                values[i] = options[i].value;
                if (options[i].value == 0.0f) continue;
                infoText.text += $"{options[i].name} : {options[i].value}\n";
            }

            GameManager.Instance.PlayerAlterationData.alterationStatus.SetStatus(values);
        }

        private void UpdateInfo(string newName, float newValue, string prevName, float prevValue)
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
