using Photon.Pun.Demo.Procedural;
using System;
using UnityEngine;
using UnityEngine.UI;

public class UIFactorSlot : MonoBehaviour
{
    public enum EType
    {
        ALPHA,
        BETA,
        MAX
    }

    [SerializeField] private Image backImg;
    [SerializeField] private Image lockImg;
    public bool bLock = true;
    public int slotNum { get;  private set; }

    public void Initialize(EType type, bool bConfirmed, int slotIndex)
    {
        slotNum = slotIndex;
        bLock = !bConfirmed;
        lockImg.enabled = !bConfirmed;

        switch (type)
        {
            case EType.ALPHA:
                backImg.color = new Color(1, 0.54f, 0.54f);
                break;
            case EType.BETA:
                backImg.color = new Color(0.54f, 0.54f, 1);
                break;
            default:
                backImg.color = new Color(1, 1, 1);
                break;
        }
    }
}
