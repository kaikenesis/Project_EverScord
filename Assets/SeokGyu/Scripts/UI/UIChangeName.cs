using System;
using TMPro;
using UnityEngine;

namespace EverScord
{
    public class UIChangeName : MonoBehaviour
    {
        [SerializeField] private TMP_InputField userNameInput;
        private string userName;

        public static Action<string, int> OnChangeName = delegate { };

        public void ChangeName()
        {
            if (string.IsNullOrEmpty(userName)) return;

            OnChangeName?.Invoke(userName, GameManager.Instance.userData.money);
            gameObject.SetActive(false);
        }

        public void SetUserName()
        {
            userName = userNameInput.text;
        }
    }
}
