using UnityEngine;

namespace EverScord
{
    public class UIToggleButton : MonoBehaviour
    {
        [SerializeField] private GameObject[] toggleObject;

        public void ToggleObject()
        {
            for (int i = 0; i < toggleObject.Length; i++)
            {
                toggleObject[i].SetActive(!toggleObject[i].activeSelf);
            }
        }
    }
}
