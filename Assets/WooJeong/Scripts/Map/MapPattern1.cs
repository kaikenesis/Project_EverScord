using EverScord;
using EverScord.Character;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPattern1 : MonoBehaviour
{
    [SerializeField] private bool isReverse;
    [SerializeField] private float rotateSpeed = 0.1f;
    [SerializeField] private float lenght = 32f;
    [SerializeField] private V3DLaserProgressCustom v3DLaserProgressCustom;
    [SerializeField] private float startDelay = 3f;
    [SerializeField] private GameObject effect;
    private BoxCollider boxCollider;
    private float laserDamage = 20;
    private Coroutine rotate;
    private PhotonView photonView;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        photonView = GetComponent<PhotonView>();

        photonView.RPC("SyncMapLaserActice", RpcTarget.Others, false);
        SetActiveEffect(false);

        v3DLaserProgressCustom.maxLength = lenght;
        if (isReverse)
            rotateSpeed *= -1;
    }

    private void Start()
    {
        LevelControl.OnProgressUpdated += ProgressCheck;
        if (PhotonNetwork.IsMasterClient)
        {
            rotate = StartCoroutine(Rotate());
        }
    }

    private void SetActiveEffect(bool value)
    {
        effect.SetActive(value);
        boxCollider.enabled = value;
    }

    private void OnValidate()
    {
        v3DLaserProgressCustom.maxLength = lenght;
    }

    private IEnumerator Rotate()
    {
        yield return new WaitForSeconds(startDelay);
        SetActiveEffect(true);
        photonView.RPC("SyncMapLaserActice", RpcTarget.Others, true);

        while (true)
        {
            transform.Rotate(0, rotateSpeed, 0);
            yield return new WaitForSeconds(0.05f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("MapLaser hit");
            CharacterControl control = other.GetComponent<CharacterControl>();
            control.DecreaseHP(laserDamage);
        }
    }

    [PunRPC]
    private void SyncMapLaserActice(bool tf)
    {
        SetActiveEffect(tf);
    }

    protected void ProgressCheck(float currentProgress)
    {
        if (currentProgress == 1)
        {
            // 현재 진행도 체크하고 다 됐으면 죽임
           gameObject.SetActive(false);
        }
    }
}
