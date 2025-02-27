using EverScord.Character;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class V3DLaserCustom : MonoBehaviour
{
    private float laserDamage = 20;

    private void OnTriggerEnter(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Laser hit");
            CharacterControl control = other.GetComponent<CharacterControl>();
            control.DecreaseHP(laserDamage);
        }
    }
}
