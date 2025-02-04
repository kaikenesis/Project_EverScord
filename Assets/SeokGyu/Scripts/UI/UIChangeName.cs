using System;
using TMPro;
using UnityEngine;

namespace EverScord
{
    public class UIChangeName : MonoBehaviour
    {
        [SerializeField] private TMP_InputField userNameInput;
        private string userName;

        public static Action OnChangeName = delegate { };

        public void ChangeName()
        {

        }

        public void SetUserName()
        {
            userName = userNameInput.text;
        }
    }
}
