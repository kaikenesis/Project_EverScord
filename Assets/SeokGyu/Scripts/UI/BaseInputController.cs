using UnityEngine;

namespace EverScord
{
    public class BaseInputController : MonoBehaviour
    {
        private void Update()
        {
            OnMouseClick();
            OnKeyInput();
        }

        protected virtual void OnMouseClick()
        {
        }

        protected virtual void OnKeyInput()
        {
        }
    }
}
