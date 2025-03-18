using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EverScord
{
    public class UIChangeName : MonoBehaviour
    {
        [SerializeField] private TMP_InputField userNameInput;
        [SerializeField] private GameObject warningText;
        [SerializeField] private Outline warningOutline;
        private string userName;
        private ToggleObject toggleObject;

        public static Action<string> OnChangeName = delegate { };

        private void Awake()
        {
            toggleObject = GetComponent<ToggleObject>();
        }

        public void ChangeName()
        {
            if (string.IsNullOrEmpty(userName))
            {
                warningOutline.enabled = true;
                warningText.SetActive(true);
                return;
            }

            OnChangeName?.Invoke(userName);

            userName = "";
            userNameInput.text = "";
            warningOutline.enabled = false;
            warningText.SetActive(false);
            toggleObject.OnDeactivateObjects();
        }

        public void SetUserName()
        {
            userName = userNameInput.text;
        }
    }
}
