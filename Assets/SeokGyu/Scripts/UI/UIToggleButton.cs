using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIToggleButton : MonoBehaviour
{
    [SerializeField] private GameObject[] toggleObject;

    private void Awake()
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
