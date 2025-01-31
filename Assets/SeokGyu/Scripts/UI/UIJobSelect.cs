using EverScord;
using System;
using UnityEngine;

public class UIJobSelect : MonoBehaviour
{
    public static Action OnChangeJob = delegate { };

    public void OnChangeToDealer()
    {
        GameManager.Instance.userData.job = EJob.DEALER;
        OnChangeJob?.Invoke();
    }

    public void OnChangeToHealer()
    {
        GameManager.Instance.userData.job = EJob.HEALER;
        OnChangeJob?.Invoke();
    }
}
