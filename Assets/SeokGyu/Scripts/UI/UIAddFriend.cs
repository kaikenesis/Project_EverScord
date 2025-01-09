using System;
using UnityEngine;

public class UIAddFriend : MonoBehaviour
{
    [SerializeField] private string displayName;

    public static Action<string> OnAddFriend = delegate { };

    public void SetAddFreindName(string name)
    {
        displayName = name;
    }

    public void AddFriend()
    {
        if (string.IsNullOrEmpty(displayName)) return;
        OnAddFriend?.Invoke(displayName);
        //
    }
}
