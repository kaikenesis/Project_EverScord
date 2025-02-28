using EverScord.Character;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLaser : MonoBehaviour
{
    [SerializeField] private bool isReverse;
    [SerializeField] private float rotateSpeed = 0.1f;
    [SerializeField] private float lenght = 32f;
    [SerializeField] private V3DLaserProgressCustom v3DLaserProgressCustom;

    private float laserDamage = 20;
    private Coroutine rotate;

    private void Awake()
    {
        v3DLaserProgressCustom.maxLength = lenght;
        if (isReverse)
            rotateSpeed *= -1;
    }

    private void OnEnable()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            rotate = StartCoroutine(Rotate());
        }
    }

    private void OnValidate()
    {
        v3DLaserProgressCustom.maxLength = lenght;

    }

    private IEnumerator Rotate()
    {

        while (true)
        {
            transform.Rotate(0, rotateSpeed, 0);
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    private void OnDisable()
    {
        StopCoroutine(rotate);
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
}
