using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace EverScord
{
    public class ResolutionController : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown resolutionDropdown;
        private List<Resolution> resolutions = new List<Resolution>();
        private int optimalResolutionIndex = 0;

        private void Awake()
        {
            resolutions.Add(new Resolution { width = 1280, height = 720 });
            resolutions.Add(new Resolution { width = 1280, height = 960 });
            resolutions.Add(new Resolution { width = 1280, height = 1024 });
            resolutions.Add(new Resolution { width = 1440, height = 1080 });
            resolutions.Add(new Resolution { width = 1600, height = 900 });
            resolutions.Add(new Resolution { width = 1600, height = 1200 });
            resolutions.Add(new Resolution { width = 1680, height = 1050 });
            resolutions.Add(new Resolution { width = 1920, height = 1080 });
            resolutions.Add(new Resolution { width = 1920, height = 1200 });

            if (resolutionDropdown != null)
            {
                resolutionDropdown.ClearOptions();

                HashSet<string> options = new HashSet<string>();

                for (int i = 0; i < resolutions.Count; i++)
                {
                    string option = resolutions[i].width + "x" + resolutions[i].height;

                    // ���� ������ �ػ󵵸� ��ǥ ǥ��
                    if (resolutions[i].width == Screen.currentResolution.width &&
                        resolutions[i].height == Screen.currentResolution.height)
                    {
                        optimalResolutionIndex = i;
                        option += " *";
                    }
                    options.Add(option);
                }

                resolutionDropdown.AddOptions(new List<string>(options));
                resolutionDropdown.value = optimalResolutionIndex;
                resolutionDropdown.RefreshShownValue();

                // ������ ���� ������ �ػ󵵷� ���۵ǵ��� ����
                SetResolution(optimalResolutionIndex);
            }
        }

        public void SetResolution(int resolutionIndex)
        {
            Resolution resolution = resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }
    }

}
