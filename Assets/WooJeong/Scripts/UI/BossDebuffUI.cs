using DG.Tweening;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossDebuffUI : MonoBehaviour
{
    [SerializeField] private GameObject slowUI;
    [SerializeField] private GameObject poisonUI;
    private Image slowImage;
    private Image poisonImage;
    private PhotonView photonView;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        slowImage = slowUI.GetComponent<Image>();
        poisonImage = poisonUI.GetComponent<Image>();
    }

    private void Start()
    {
        DebuffSystem.OnBossDebuffStart += DebuffEnter;
        DebuffSystem.OnBossDebuffEnd += DebuffEnd;
    }

    public void DebuffEnter(EBossDebuff bossDebuff)
    {
        switch (bossDebuff)
        {
            case EBossDebuff.SLOW:
                photonView.RPC(nameof(SyncBossSlowDebuffUI), RpcTarget.All, true);
                break;
            case EBossDebuff.POISON:
                photonView.RPC(nameof(SyncBossPoisonDebuffUI), RpcTarget.All, true);
                break;
        }
    }

    public void DebuffEnd(EBossDebuff bossDebuff)
    {
        switch (bossDebuff)
        {
            case EBossDebuff.SLOW:
                photonView.RPC(nameof(SyncBossSlowDebuffUI), RpcTarget.All, false);
                break;
            case EBossDebuff.POISON:
                photonView.RPC(nameof(SyncBossPoisonDebuffUI), RpcTarget.All, false);
                break;
        }
    }

    [PunRPC]
    private void SyncBossSlowDebuffUI(bool value)
    {
        slowUI.SetActive(value);
    }
    [PunRPC]
    private void SyncBossPoisonDebuffUI(bool value)
    {
        poisonUI.SetActive(value);
    }
}
