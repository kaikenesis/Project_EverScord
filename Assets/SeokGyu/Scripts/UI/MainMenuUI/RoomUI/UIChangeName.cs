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

        public static Action<string> OnChangeName = delegate { };

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
            gameObject.SetActive(false);
        }

        public void SetUserName()
        {
            userName = userNameInput.text;
        }
    }
}
