using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogInUI : MonoBehaviour
{
    public void HiddenUI()
    {
        gameObject.SetActive(false);
    }

    public void VisibleUI()
    {
        gameObject.SetActive(true);
    }
}
