using UnityEngine;

namespace EverScord
{
    public class UIToggleButton : MonoBehaviour
    {
        [SerializeField] protected GameObject[] toggleObject;

        public virtual void ToggleObject()
        {
            for (int i = 0; i < toggleObject.Length; i++)
            {
                toggleObject[i].SetActive(!toggleObject[i].activeSelf);
            }
        }
    }
}
