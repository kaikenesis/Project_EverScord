using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEditor.Progress;

namespace EverScord
{
    public class UIDisplayApplyInfo : MonoBehaviour
    {
        [SerializeField] private TMP_Text infoText;

        private List<OptionInfo> options = new List<OptionInfo>();

        private void Awake()
        {
            UIFactorOptionList.OnInitializeOptionName += HandleInitializeOptionName;
        }

        private void OnDestroy()
        {
            UIFactorOptionList.OnInitializeOptionName -= HandleInitializeOptionName;
        }

        private void OnEnable()
        {
            UpdateInfo();
        }

        private void HandleInitializeOptionName(string name)
        {
            OptionInfo optionInfo = new OptionInfo(name, 0.0f);
            options.Add(optionInfo);
            Debug.Log(options.Count);
        }



        private void UpdateInfo()
        {
            infoText.text = "";
            for (int i = 0; i < options.Count; i++)
            {
                if (options[i].value == 0.0f) continue;
                infoText.text += $"{options[i].name} : {options[i].value}";
            }
        }

        public class OptionInfo
        {
            public string name;
            public float value;

            public OptionInfo(string name, float value)
            {
                this.name = name;
                this.value = value;
            }
        }
    }
}
