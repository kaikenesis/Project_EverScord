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
    private float damage = 20;
    private Coroutine rotate;
    private PhotonView photonView;
    private Dictionary<GameObject, float> hitPlayers = new();

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        photonView = GetComponent<PhotonView>();
        boxCollider.size = new Vector3(boxCollider.size.x, boxCollider.size.y, lenght);
        boxCollider.center = new Vector3(0, 0, lenght / 2);
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

    private void OnTriggerStay(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        if (other.gameObject.CompareTag("Player"))
        {
            if (!hitPlayers.ContainsKey(other.gameObject))
            {
                hitPlayers.Add(other.gameObject, 1);
            }
            else if (hitPlayers[other.gameObject] > 0)
            {
                hitPlayers[other.gameObject] -= Time.deltaTime;
                return;
            }

            hitPlayers[other.gameObject] = 1f;
            CharacterControl controller = other.GetComponent<CharacterControl>();
            controller.DecreaseHP(damage);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        hitPlayers.Remove(other.gameObject);
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
