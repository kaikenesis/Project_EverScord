using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EverScord
{
    public class UIDisplayContainor : MonoBehaviour
    {
        [SerializeField] private GameObject characterContainor;
        [SerializeField] private GameObject jobContainor;

        private void Awake()
        {
            characterContainor.gameObject.SetActive(false);
            jobContainor.gameObject.SetActive(false);
        }

        public void OnToggleCharacter()
        {
            characterContainor.SetActive(!characterContainor.activeSelf);
        }

        public void OnToggleJob()
        {
            jobContainor.SetActive(!jobContainor.activeSelf);
        }
    }
}
