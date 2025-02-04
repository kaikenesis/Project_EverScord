using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace EverScord
{
    public class ResolutionController : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown resolutionDropdown;
        private Resolution[] resolutions;

        private void Awake()
        {
            resolutions = Screen.resolutions;

            if(resolutionDropdown != null)
            {
                resolutionDropdown.ClearOptions();

                HashSet<string> options = new HashSet<string>();

                int curResolutionIndex = 0;
                for (int i = 0; i < resolutions.Length; i++)
                {
                    string option = resolutions[i].width + "x" + resolutions[i].height;
                    options.Add(option);

                    if (resolutions[i].width == Screen.currentResolution.width &&
                        resolutions[i].height == Screen.currentResolution.height)
                    {
                        curResolutionIndex = i;
                    }
                }

                resolutionDropdown.AddOptions(new List<string>(options));
                resolutionDropdown.value = curResolutionIndex;
                resolutionDropdown.RefreshShownValue();
            }
        }

        public void SetResolution(int resolutionIndex)
        {
            Resolution resolution = resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }
    }

}
