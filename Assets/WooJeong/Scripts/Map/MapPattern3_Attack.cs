using EverScord.Character;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPattern3_Attack : MonoBehaviour
{
    private float attackDamage = 10;
    private SphereCollider sphereCollider;
    private Dictionary<GameObject, float> hitPlayers = new();

    private void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
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
            controller.DecreaseHP(attackDamage);
        }
    }
}
