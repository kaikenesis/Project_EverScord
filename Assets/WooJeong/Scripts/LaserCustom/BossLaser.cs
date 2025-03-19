using EverScord.Character;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossLaser : MonoBehaviour
{
    private float laserDamage = 20;
    private Dictionary<GameObject, float> hitPlayers = new();
    private float tickTime = 0.5f;
    private float baseAttack;

    public void SetDamage(float damage, float attack)
    {
        laserDamage = damage;
        baseAttack = attack;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        if (other.gameObject.CompareTag("Player"))
        {
            if (!hitPlayers.ContainsKey(other.gameObject))
            {
                hitPlayers.Add(other.gameObject, tickTime);
            }
            else if (hitPlayers[other.gameObject] > 0)
            {
                hitPlayers[other.gameObject] -= Time.deltaTime;
                return;
            }

            hitPlayers[other.gameObject] = tickTime;
            CharacterControl controller = other.GetComponent<CharacterControl>();
            controller.DecreaseHP(laserDamage);
        }
    }
}
