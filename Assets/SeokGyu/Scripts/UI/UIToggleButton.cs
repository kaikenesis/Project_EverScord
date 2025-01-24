using UnityEngine;

namespace EverScord
{
    public class UIToggleButton : MonoBehaviour
    {
        [SerializeField] private GameObject[] toggleObject;

        private void Awake()
        {
            Init();
        }

        public void Init()
        {
            for (int i = 0; i < toggleObject.Length; i++)
            {
                toggleObject[i].SetActive(false);
            }
        }

        public void ToggleObject()
        {
            for (int i = 0; i < toggleObject.Length; i++)
            {
                toggleObject[i].SetActive(!toggleObject[i].activeSelf);
            }
        }
    }
}
