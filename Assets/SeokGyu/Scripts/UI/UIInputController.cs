using UnityEngine;

public class UIInputController : MonoBehaviour
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
