using EverScord.Character;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPattern3_Attack : MonoBehaviour
{
    [SerializeField] float tickDistance = 1f;
    private float attackDamage = 2;
    private SphereCollider sphereCollider;
    private Dictionary<GameObject, float> hitPlayers = new();

    private void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        if (other.gameObject.CompareTag("Player"))
        {
            if (!hitPlayers.ContainsKey(other.gameObject))
            {
                hitPlayers.Add(other.gameObject, 0);
            }
            else if (hitPlayers[other.gameObject] > 0)
            {
                hitPlayers[other.gameObject] -= Time.deltaTime;
                return;
            }

            hitPlayers[other.gameObject] = tickDistance;
            CharacterControl controller = other.GetComponent<CharacterControl>();
            controller.DecreaseHP(attackDamage, true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(!PhotonNetwork.IsMasterClient) return;

        hitPlayers.Remove(other.gameObject);
    }
}
